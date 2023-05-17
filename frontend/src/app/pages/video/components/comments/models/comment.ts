import { FormControl } from '@angular/forms';
import { CommentResponse } from './comment-response';

export interface Comment {
  id: string;
  authorId: string;
  content: string;
  avatarImage: string;
  nickname: string;
  hasResponses: boolean;
  responses: CommentResponse[] | null;
  isResponsesVisible: boolean;
  isResponseGettingAdded: boolean;
  responseFormControl: FormControl;
}
