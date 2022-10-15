import { HttpClient, HttpParams } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { Movie } from './Movie';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  private apiKey:string = '780d08aa64c5fc6d341be3ae5e52fe8b'
  //https://api.themoviedb.org/3/search/movie?api_key=780d08aa64c5fc6d341be3ae5e52fe8b&query=batman

  private baseApiUrl:string = `https://api.themoviedb.org/3/search/movie`
  private selectedMovie$:Subject<Movie> = new Subject<Movie>()
  constructor(private http:HttpClient) { }
  ngOnInit(){
    
  }
  get currentMovie(){
    return this.selectedMovie$
  }
  searchMovie(query:string){
    const params = new HttpParams().set("api_key",this.apiKey).set('query',query)
    const resultss = this.http.get<any[]>(this.baseApiUrl,{params}).subscribe(response =>{
        console.log(response);
        
      
    })

    
    return resultss
  }

  changeSelected(movie:Movie){
    this.selectedMovie$.next(movie)
  }
}
