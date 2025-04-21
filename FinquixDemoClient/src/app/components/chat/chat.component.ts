import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Question } from '../../../models/question';
import { ChatService } from '../../../services/components/chat.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { UserQuery } from '../../../models/userQuery';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CryptoMarketService } from '../../../services/components/crypto-market.service';
import { StockMarketService } from '../../../services/components/stock-market.service';
import { map, Subject, switchMap, takeUntil } from 'rxjs';
import { Answer } from '../../../models/answer';
import { StructuredAnswer } from '../../../models/structuredAnswer';
import { MarketDataStoreService } from '../../../services/shared/market-data-store.service';

@Component({
  selector: 'app-chat',
  imports: [
    FormsModule,
    NgIf,
    NgFor,
    NgClass,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
  standalone: true,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit, OnDestroy {
  isOpen = false;
  isLoading = false;

  userId: number = 1;
  userInput: string = '';
  messages: Array<{
    type: 'user' | 'ai';
    text: string | (Answer & { showDetails?: boolean });
  }> = [];

  questions: Question[] = [];
  showQuestions = true;
  questionsCollapsed = false;

  private destroy$ = new Subject<void>();

  constructor(
    private chatService: ChatService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private stockMarketService: StockMarketService,
    private cryptoMarketService: CryptoMarketService,
    private store: MarketDataStoreService
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.userId = +params['userId'] || 1;
    });
    this.fetchQuestions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  fetchQuestions(): void {
    this.chatService.getQuestions().subscribe({
      next: data => this.questions = data,
      error: error => console.error('Error fetching questions:', error)
    });
  }

  // Add these methods to your component class
  getUniqueCategories(): string[] {
    return [...new Set(this.questions.map(q => q.category))].filter(c => c);
  }

  getQuestionsByCategory(category: string): Question[] {
    return this.questions.filter(q => q.category === category);
  }

  toggleQuestionsCollapse(): void {
    this.showQuestions = !this.showQuestions;
  }

  selectQuestion(questionText: string): void {
    this.userInput = questionText;
    // Optional: auto-focus the input field
    setTimeout(() => {
      const input = document.querySelector('.chat-input input');
      if (input) {
        (input as HTMLInputElement).focus();
      }
    }, 0);
  }

  // USE THIS for auto-send on click
  // selectQuestion(questionText: string): void {
  //   this.userInput = questionText;
  //   // Optional: auto-send after short delay
  //   setTimeout(() => {
  //     this.sendUserInput();
  //   }, 300);
  // }

  isAnswer(text: string | Answer): text is Answer {
    return typeof text !== 'string' && 'summary' in text && Array.isArray(text.details);
  }

  normalizeApiResponse(response: any): Answer {
    try {
      // If it's a stringified JSON object
      if (typeof response === 'string' && response.includes('"summary"')) {
        response = JSON.parse(response);
      }

      // If response.summary is actually a JSON object
      if (typeof response.summary === 'string' && response.summary.includes('"summary"')) {
        const maybeNested = JSON.parse(response.summary);
        if (maybeNested.summary && maybeNested.details) {
          response = maybeNested;
        }
      }
    } catch (e) {
      console.error('Parsing fallback error:', e);
    }

    if (!response || typeof response !== 'object') {
      return {
        summary: '⚠️ Could not parse response.',
        details: [{
          section: 'Error',
          content: [String(response)]
        }],
        showDetails: false
      };
    }

    if (!Array.isArray(response.details)) {
      response.details = [{
        section: 'Details',
        content: [String(response.details || 'No details provided')]
      }];
    }

    return {
      summary: response.summary,
      details: response.details,
      showDetails: false
    };
  }

  getAiAnswer(message: any): Answer & { showDetails?: boolean } | null {
    if (typeof message === 'object' && message !== null && 'summary' in message) {
      return message as Answer & { showDetails?: boolean };
    }
    return null;
  }

  sendUserInput(): void {
    const input = this.userInput.trim();
    if (!input) return;

    this.messages.push({ type: 'user', text: input });
    this.userInput = '';
    this.scrollToBottom();
    this.isLoading = true;

    const stocks = this.store.stockAssets;
    const crypto = this.store.cryptoAssets;

    if (!stocks.length || !crypto.length) {
      console.warn('Market data not ready. Please wait for it to load before asking.');
      this.isLoading = false;
      this.messages.push({
        type: 'ai',
        text: {
          summary: '⚠️ Market data not available yet.',
          details: [{
            section: 'Info',
            content: ['Please wait for stock and crypto data to load before asking a question.']
          }],
          showDetails: false
        }
      });
      return;
    }

    this.processQuestionWithContext(input, stocks, crypto);
  }

  sendUserInputOLD(): void {
    const input = this.userInput.trim();
    if (!input) return;

    this.messages.push({ type: 'user', text: input });
    this.userInput = '';
    this.scrollToBottom();
    this.isLoading = true;

    this.stockMarketService.getStockAssets().pipe(
      takeUntil(this.destroy$),
      switchMap(stocks =>
        this.cryptoMarketService.getCryptoAssets().pipe(
          takeUntil(this.destroy$),
          map(crypto => [stocks, crypto])
        )
      )
    ).subscribe({
      next: ([stocks, crypto]) => {
        this.processQuestionWithContext(input, stocks, crypto);
      },
      error: (error) => {
        console.error('Error fetching market data:', error);
        this.processQuestionWithoutContext(input);
      }
    });
  }

  private processQuestionWithContext(
    input: string,
    stocks: any[],
    crypto: any[]
  ): void {
    const userQueryWithContext = {
      userId: this.userId,
      questionText: input,
      currentStockData: stocks,
      currentCryptoData: crypto
    };

    this.chatService.askQuestionWithContext(userQueryWithContext).subscribe({
      next: (response: Answer) => this.handleAiResponse(response),
      error: (error) => this.handleErrorResponse(error)
    });
  }

  private processQuestionWithoutContext(input: string): void {
    const userQuery: UserQuery = {
      userId: this.userId,
      questionText: input
    };

    this.chatService.askQuestion(userQuery).subscribe({
      next: (response: Answer) => this.handleAiResponse(response),
      error: (error) => this.handleErrorResponse(error)
    });
  }

  isStructuredAnswer(text: string | StructuredAnswer): text is StructuredAnswer {
    return typeof text !== 'string' && 'summary' in text && Array.isArray((text as StructuredAnswer).details);
  }

  getStructuredAnswer(text: string | StructuredAnswer): StructuredAnswer | null {
    if (this.isStructuredAnswer(text)) {
      return text;
    }
    return null;
  }

  private handleAiResponse(response: StructuredAnswer): void {
    const normalizedResponse = this.normalizeApiResponse(response);

    if (!normalizedResponse || !normalizedResponse.summary) {
      this.messages.push({
        type: 'ai',
        text: {
          summary: '⚠️ No valid answer received.',
          details: [{
            section: 'Notice',
            content: ['The assistant could not process your question. Please try again or ask something different.']
          }],
          showDetails: false
        }
      });
    } else {
      this.messages.push({
        type: 'ai',
        text: normalizedResponse
      });
    }

    this.isLoading = false;
    this.scrollToBottom();
  }

  private handleErrorResponse(error: any): void {
    console.error('Error getting AI response:', error);
    this.messages.push({
      type: 'ai',
      text: {
        summary: 'Error: Could not get AI response',
        details: error.message,
        showDetails: false
      }
    });
    this.isLoading = false;
    this.scrollToBottom();
  }

  toggleDetails(answer: Answer): void {
    answer.showDetails = !answer.showDetails;
    this.scrollToBottom();
  }

  resetChat(): void {
    this.messages = [];
    this.cdr.detectChanges();
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      setTimeout(() => this.scrollToBottom(), 50);
    }
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const messagesContainer = document.querySelector('.chat-messages');
      if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
      }
    });
  }
}