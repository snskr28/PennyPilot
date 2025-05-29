import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MATERIAL_IMPORTS } from '../shared/material';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-auth-container',
  imports: [CommonModule,LoginComponent,SignupComponent,RouterModule,...MATERIAL_IMPORTS],
  templateUrl: './auth-container.component.html',
  styleUrl: './auth-container.component.scss'
})
export class AuthContainerComponent implements OnInit, OnDestroy {
  selectedTabIndex = 0;
  
  // Slideshow properties
  currentSlide = 0;
  slides = [0, 1, 2];
  private slideInterval: any;

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
}
