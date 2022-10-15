import { Component, OnInit } from '@angular/core';

import {Movie} from '../Movie'
import { MovieService } from '../movie.service';
import {data} from '../mock-data'

@Component({
  selector: 'search-movie',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  search:string=''
  searchResults:Movie[] = data
  constructor(private movieService:MovieService) { }

  ngOnInit(): void {

  }
  searchQuery(query:string){
    if(query.length>0){
     /* console.log(this.movieService.searchMovie(query).subscribe((results)=>{
        this.searchResults = results
      }));
      console.log(this.searchResults);
      */
     this.movieService.searchMovie(query)
    }
      
      
  }
  setCurrentMovie(movie:Movie){
      this.movieService.changeSelected(movie)
  }

}
