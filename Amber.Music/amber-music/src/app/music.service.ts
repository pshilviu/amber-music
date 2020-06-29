import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, map } from 'rxjs/operators';
import { AppSettings } from "../app.settings";
import { ArtistSearchResult } from "../domain/artist-search-result";
import { ArtistWorkReport } from "../domain/artist-work-report";
import { ArtistSearch } from 'src/domain/artist-search';
import { ArtistReleaseReport } from 'src/domain/artist-release-report';

@Injectable({
  providedIn: 'root'
})

export class MusicService {

  constructor(
    private settings: AppSettings,
    private http: HttpClient) { }
  
  // TODO: change to POST along with the backend
  searchArtists(searchTerm: string) : Observable<ArtistSearchResult> {
    let url = this.settings.musicApiUrl + '/find/' + searchTerm;
    return this.http.get<ArtistSearchResult>(url).pipe(
      map((response) => { return response; }),
      catchError(this.handleError)
    );
  }

  lastSearchedArtists() : Observable<ArtistSearch[]> {
    let url = this.settings.musicApiUrl + '/lastsearches/';
    return this.http.get<ArtistSearch[]>(url).pipe(
      map((response) => { return response; }),
      catchError(this.handleError)
    );
  }

  getArtistReport(artistId: string) : Observable<ArtistWorkReport> {
    let url = this.settings.musicApiUrl + '/report/' + artistId;
    return this.http.get<ArtistWorkReport>(url).pipe(
      map((response) => { return response; }),
      catchError(this.handleError)
    );
  }

  getReleasesReport(artistId: string) : Observable<ArtistReleaseReport> {
    let url = this.settings.musicApiUrl + '/releases/' + artistId;
    return this.http.get<ArtistReleaseReport>(url).pipe(
      map((response) => { return response; }),
      catchError(this.handleError)
    );
  }

  private handleError(error: Response) {
    console.log("handleError")
    let errMsg: string;
    errMsg = "error";
    return Observable.throw(errMsg);
  }
}
