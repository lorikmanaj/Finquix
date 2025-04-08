// export interface Answer {
//     summary: string;
//     details?: string;
// }

import { AnswerSection } from "./answerSection";

export interface Answer {
    summary: string;
    details: AnswerSection[];
    showDetails?: boolean;
}