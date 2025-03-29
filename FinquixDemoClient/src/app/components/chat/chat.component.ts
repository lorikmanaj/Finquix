import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Answer } from '../../../models/answer';
import { Question } from '../../../models/question';
import { ChatService } from '../../../services/components/chat.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { UserQuery } from '../../../models/userQuery';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, NgIf, NgFor, NgClass],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit {
  isOpen = false;
  isLoading = false;
  userId: number = 1;
  userInput: string = '';

  questions: Question[] = [];

  messages: {
    type: 'user' | 'ai';
    text: string | (Answer & { showDetails?: boolean });
  }[] = [];

  constructor(private chatService: ChatService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.userId = +params['userId'] || 1;
    });

    this.fetchQuestions();
  }

  fetchQuestions(): void {
    this.chatService.getQuestions().subscribe({
      next: data => (this.questions = data),
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

    const userQuery: UserQuery = {
      userId: this.userId,
      question: {
        id: 0,
        category: 'custom',
        text: input
      }
    };

    this.userInput = '';
    this.sendQuestion(userQuery);
  }

  sendQuestion(userQuery: UserQuery): void {
    this.isLoading = true;

    this.chatService.askQuestion(userQuery).subscribe({
      next: (response: Answer) => {
        this.messages.push({
          type: 'ai',
          text: {
            summary: response.summary,
            details: response.details,
            showDetails: false
          }
        });

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Error getting AI response:', error);
        this.messages.push({ type: 'ai', text: 'Error: Unexpected response from AI' });
        this.isLoading = false;
      }
    });
  }

  // toggleDetails(message: any): void {
  //   message.text.showDetails = !message.text.showDetails;
  //   this.cdr.detectChanges();
  // }
  toggleDetails(answer: Answer & { showDetails?: boolean }) {
    answer.showDetails = !answer.showDetails;
  }

  resetChat(): void {
    this.messages = [];
    this.cdr.detectChanges(); // Ensure UI resets properly
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
    this.cdr.detectChanges(); // Ensure the UI updates when toggled
  }
}
