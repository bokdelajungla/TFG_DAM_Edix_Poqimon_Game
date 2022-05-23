import { Component } from '@angular/core';

@Component({
  selector: 'app-instructions-page',
  templateUrl: './instructions-page.component.html',
  styleUrls: ['./instructions-page.component.scss']
})
export class InstructionsPageComponent {

  public pdfSrc = "../../../assets/docs/gdd.pdf";

  ngOnInit(): void {

  }

}
