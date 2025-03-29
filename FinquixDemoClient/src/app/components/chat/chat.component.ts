import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Answer } from '../../../models/answer';
import { Question } from '../../../models/question';
import { ChatService } from '../../../services/components/chat.service';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { UserQuery } from '../../../models/userQuery';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [NgIf, NgFor, NgClass],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit {
  isOpen = false;
  questions: Question[] = [];
  messages: { type: 'user' | 'ai'; text: string }[] = [];
  isLoading = false;

  userId: number = 1; // default to 1 if missing

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
      next: (data) => (this.questions = data),
      error: (error) => console.error('Error fetching questions:', error)
    });
  }

  sendQuestion(userQuery: UserQuery): void {
    this.isLoading = true;
    this.messages.push({ type: 'user', text: userQuery.question.text });

    this.chatService.askQuestion(userQuery).subscribe({
      next: (response: Answer) => {
        console.log('API Response:', response);

        if (response && response.answerDS) {
          this.messages.push({ type: 'ai', text: response.answerDS });
        } else {
          console.error('Unexpected API response format:', response);
          this.messages.push({ type: 'ai', text: 'Error: Unexpected response from AI' });
        }

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error getting AI response:', error);
        this.isLoading = false;
      }
    });
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
