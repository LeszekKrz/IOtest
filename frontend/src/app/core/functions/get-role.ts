export function getRole(): string {
    const role = localStorage.getItem('role');
    if (role === null) {
      return '';
    }
    return role;
  }
  