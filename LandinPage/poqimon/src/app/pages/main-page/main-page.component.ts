import { Component } from '@angular/core';
import { NavigateService } from 'src/app/services/navigate/navigate.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.scss']
})
export class MainPageComponent {

  constructor(private navigateService: NavigateService) { }

  onCickGoDownload() {
    this.navigateService.goToPage("download");
  }

}
