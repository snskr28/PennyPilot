import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthContainerComponent } from "./auth/auth-container.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'PennyPilot.Frontend';
}
