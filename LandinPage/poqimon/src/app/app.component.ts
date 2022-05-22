import { Component } from '@angular/core';
import { Subject } from 'rxjs/internal/Subject';
import { ApiService } from './services/api/api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  destroyed = new Subject<void>(); // Detectar destrucción del componente

  appConfigLoaded: boolean = false; // Si el appConfig está cargado
  translateLoaded: boolean = false; // Traducciones cargadas

  constructor(
    private apiService: ApiService) { }



}
