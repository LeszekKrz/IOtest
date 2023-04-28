export function getTimeAgo(value: Date): string {
  value = new Date(value);
  const now = new Date();
  const timeDiffInSeconds = Math.floor((now.getTime() - value.getTime()) / 1000);
  const minutes = Math.floor(timeDiffInSeconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  const weeks = Math.floor(days / 7);

  if (weeks > 0) {
    return weeks + (weeks === 1 ? ' week' : ' weeks');
  } else if (days > 0) {
    return days + (days === 1 ? ' day' : ' days');
  } else if (hours > 0) {
    return hours + (hours === 1 ? ' hour' : ' hours');
  } else {
    const roundedMinutes = Math.max(1, minutes);
    return roundedMinutes + (roundedMinutes === 1 ? ' minute' : ' minutes');
  }
}
