import { Question } from "./question";

export interface UserQuery {
    userId: number;
    question: Question;
}