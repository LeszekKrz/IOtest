export interface UserDTO {
    id: string;
    email: string;
    nickname: string;
    name: string;
    surname: string;
    accountBalance: number | null;
    userType: string;
    avatarImage: string | null;
    subscriptionsCount: number | null;
  }
