import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { catchError, Observable } from 'rxjs';
import { ErrorResponseDTO } from '../models/error-response-dto';
import { insuficcientAccessMessage, internalServerErrorMessage, notFoundMessage, unauthorizedMessage } from '../messages';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private messageService: MessageService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((errorResponse: HttpErrorResponse) => {
        switch(errorResponse.status) {
          case 400:
            const error: ErrorResponseDTO[] = errorResponse.error;
            error.forEach((e: ErrorResponseDTO) => {
              this.messageService.add({severity: 'error', summary: 'Error', detail: e.errorMessage});
            });
            break;
          case 401:
            if (errorResponse.error.errorMessage) {
              this.messageService.add({severity: 'error', summary: 'Error', detail: errorResponse.error.errorMessage});
            }
            else {
              this.messageService.add({severity: 'error', summary: 'Error', detail: unauthorizedMessage});
            }
            break;
          case 403:
            this.messageService.add({severity: 'error', summary: 'Error', detail: insuficcientAccessMessage})
            break;
          case 404:
              this.messageService.add({severity: 'error', summary: 'Error', detail: notFoundMessage})
              break;
          case 422:
            this.messageService.add({severity: 'error', summary: 'Error', detail: errorResponse.error.title});
            break;
          default:
            this.messageService.add({severity: 'error', summary: 'Error', detail: internalServerErrorMessage});
        }
        throw errorResponse.error;
      })
    );
  }
}
