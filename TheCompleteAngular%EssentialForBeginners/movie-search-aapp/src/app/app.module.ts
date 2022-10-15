import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserModule } from '@angular/platform-browser';
import { DisplayMovieComponent } from './display-movie/display-movie.component';
import { FooterComponent } from './footer/footer.component';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from './header/header.component';
import  {HttpClientModule} from '@angular/common/http'
import { MoviePreviewComponent } from './search/movie-preview/movie-preview.component';
import { NgModule } from '@angular/core';
import { SearchComponent } from './search/search.component';
@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    DisplayMovieComponent,
    SearchComponent,
    MoviePreviewComponent,
    FooterComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule ,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
