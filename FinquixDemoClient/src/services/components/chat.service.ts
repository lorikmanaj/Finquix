import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Answer } from '../../models/answer';
import { Question } from '../../models/question';
import { ApiService } from '../global/api.service';
import { UserQuery } from '../../models/userQuery';

@Injectable({
  providedIn: 'root'
})
export class ChatService extends ApiService {

  getQuestions(): Observable<Question[]> {
    return this.get<Question[]>('DSChatCon/questions');
  }

  askQuestion(userQuery: UserQuery): Observable<Answer> {
    return this.post<Answer, UserQuery>('DSChatCon/ask', userQuery);
  }
}