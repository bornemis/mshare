import { BrowserModule } from '@angular/platform-browser';
import { BotDetectCaptchaModule } from 'angular-captcha';
import { RecaptchaModule } from 'angular-google-recaptcha';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomepageComponent } from './components/homepage/homepage.component';
import { AboutComponent } from './components/about/about.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { PageFooterComponent } from './components/page-footer/page-footer.component';
import { GroupManagerComponent } from './components/group-manager/group-manager.component';
import { GroupDetailComponent } from './components/group-detail/group-detail.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomepageComponent,
    AboutComponent,
    PageHeaderComponent,
    PageFooterComponent,
    GroupManagerComponent,
    GroupDetailComponent
  ],
  imports: [
    NgbModule,
    HttpClientModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    RecaptchaModule.forRoot({
      siteKey: '6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI',
    }),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
