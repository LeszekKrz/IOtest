export interface UserForRegistrationDTO {
  email: string;
  nickname: string;
  name: string;
  surname: string;
  password: string;
  userType: string;
  avatarImage: string | null;
}