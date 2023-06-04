import { environment } from "src/environments/environment";

export function getApiUrl(): string {
    const api = localStorage.getItem('api');
    if (api === '10') {
      return environment.group10WebApiUrl;
    }
    else if (api === '11') {
        return environment.group11WebApiUrl;
    }
    else if (api === '12') {
        return environment.group12WebApiUrl;
    }
    else if (api === '13') {
        return environment.group13WebApiUrl;
    }

    return environment.group11WebApiUrl;
}
  