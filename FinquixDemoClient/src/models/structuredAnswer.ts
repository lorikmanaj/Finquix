import { AnswerSection } from "./answerSection";

export interface StructuredAnswer {
    summary: string;
    details: string | AnswerSection[];
    showDetails?: boolean;
}