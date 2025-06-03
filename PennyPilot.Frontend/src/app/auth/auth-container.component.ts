import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../shared/material';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-auth-container',
  imports: [
    CommonModule,
    LoginComponent,
    SignupComponent,
    RouterModule,
    ...MATERIAL_IMPORTS
  ],
  templateUrl: './auth-container.component.html',
  styleUrl: './auth-container.component.scss'
})
export class AuthContainerComponent implements OnInit, OnDestroy {
  selectedTabIndex = 0;
  currentSlide = 0;
  slides = [0, 1, 2];
  private slideInterval: any;

  constructor(private router: Router) {
    // Sync tabs with route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.selectedTabIndex = this.router.url.includes('signup') ? 1 : 0;
    });
  }

  ngOnInit() {
    this.startSlideshow();
  }

  ngOnDestroy() {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }

  onTabChange(event: any) {
    this.selectedTabIndex = event.index;
    // Navigate to corresponding route
    this.router.navigate([event.index === 0 ? '/login' : '/signup']);
  }

  // Slideshow methods
  startSlideshow() {
    this.slideInterval = setInterval(() => {
      this.nextSlide();
    }, 4000);
  }

  nextSlide() {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }

  setSlide(index: number) {
    this.currentSlide = index;
    // Restart slideshow timer
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
      this.startSlideshow();
    }
  }
  
  isLoginOrSignupRoute(): boolean {
    return this.router.url === '/login' || this.router.url === '/signup' || this.router.url === '/';
  }
}
