import { InstructionsPageComponent } from './pages/instructions-page/instructions-page.component';
import { DownloadPageComponent } from './pages/download-page/download-page.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'home', component: MainPageComponent },
  { path: 'download', component: DownloadPageComponent },
  { path: 'instructions', component: InstructionsPageComponent },

  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
