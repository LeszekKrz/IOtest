export interface CommentDTO {
  id: string;
  authorId: string;
  content: string;
  avatarImage: string;
  nickname: string;
  hasResponses: boolean;
}
