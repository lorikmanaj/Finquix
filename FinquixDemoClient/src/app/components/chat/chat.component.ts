import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Answer } from '../../../models/answer';
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

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    FormsModule,
    NgIf,
    NgFor,
    NgClass,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
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

  private destroy$ = new Subject<void>();

  constructor(
    private chatService: ChatService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private stockMarketService: StockMarketService,
    private cryptoMarketService: CryptoMarketService
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

  isAnswer(text: string | { summary: string; details?: string; showDetails?: boolean }): text is Answer & { showDetails?: boolean } {
    return typeof text !== 'string' && 'summary' in text;
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

  private handleAiResponse(response: Answer): void {
    this.messages.push({
      type: 'ai',
      text: {
        summary: response.summary || "No response received",
        details: response.details,
        showDetails: false
      }
    });
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

  toggleDetails(answer: Answer & { showDetails?: boolean }): void {
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