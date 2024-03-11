import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  registerForm: FormGroup = new FormGroup({});
  isLogin: boolean = true;
  isPasswordHidden: boolean = true;

  constructor(
    private accountService: AccountService,
    private router: Router,
    private fb: FormBuilder,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(15)]]
    });

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(15)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(15)]]
    });
  }

  login(): void {
    this.accountService.logIn(this.loginForm.value).subscribe({
      next: (response) => {
        this.router.navigate(['dashboard']);
      },
      error: error => { this.toastr.error(error.error) }
    });
  }

  register(): void {
    this.accountService.registerUser(this.registerForm.value).subscribe({
      next: (response) => {
        this.toastr.success('User created!');
        this.router.navigate(['dashboard']);
      },
      error: error => { this.toastr.error(error.error) }
    });
  }

  navigateLoginRegister() : void {
    this.isLogin = !this.isLogin;
    this.isPasswordHidden = true;
  }

  toDo(): void {
    this.toastr.warning('Need to implement!')
  }
}
