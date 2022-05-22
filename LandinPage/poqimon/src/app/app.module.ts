import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { DownloadPageComponent } from './pages/download-page/download-page.component';
import { InstructionsPageComponent } from './pages/instructions-page/instructions-page.component';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';
import { PdfViewerModule } from 'ng2-pdf-viewer';

export const TRANSLATEMODULE = TranslateModule.forRoot({
  loader: {
    provide: TranslateLoader,
    useFactory: genericTranslateLoader,
    deps: [HttpClient]
  }
});

@NgModule({
  declarations: [
    AppComponent,
    MainPageComponent,
    DownloadPageComponent,
    InstructionsPageComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    PdfViewerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

// required for AOT compilation
export function genericTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}
