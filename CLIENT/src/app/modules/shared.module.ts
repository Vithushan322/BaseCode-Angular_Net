import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ToastrModule } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatIconModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top-right'
    }),
    NgxSpinnerModule.forRoot({
      type: 'ball-newton-cradle'
    }),
  ],
  exports: [
    ToastrModule,
    NgxSpinnerModule,
    MatIconModule
  ]
})

export class SharedModule { }
