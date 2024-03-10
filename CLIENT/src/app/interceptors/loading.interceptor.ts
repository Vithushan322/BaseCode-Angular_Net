import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize, identity } from 'rxjs';
import { SpinnerService } from '../services/spinner.service';
import { environment } from '../environments/environment';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private spinnerService: SpinnerService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.spinnerService.busy()
    
    return next.handle(request).pipe(
      (environment.production ? identity : delay(100)),
      finalize(() => {
        this.spinnerService.idle();
      })
    );
  }
}
