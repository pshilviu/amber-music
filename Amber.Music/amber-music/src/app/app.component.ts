import { Component, Inject, OnInit } from '@angular/core';
import { MusicService } from './music.service';
import { Observable, of, Subject, forkJoin } from 'rxjs';
import { ArtistSearch } from '../domain/artist-search';
import { CompareRow } from '../domain/compare-row';
import { LyricsDialog } from './lyrics-dialog.component'
import { catchError, map } from 'rxjs/operators';
import { ArtistSearchResult } from '../domain/artist-search-result';
import { ArtistWorkReport } from 'src/domain/artist-work-report';
import { MatDialog } from '@angular/material/dialog';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';

// TODO: this component must be split into multiple ones

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Amber Music';
  searchTerm: string = '';

  loadingSearchResults: boolean = false;
  searchResults: ArtistSearchResult;
  searchError$ = new Subject<boolean>();

  loadingLastSearchedArtists: boolean = false;
  lastSearchedArtists: ArtistSearch[];
  lastSearchError$ = new Subject<boolean>();

  loadingReport: boolean = false;
  selectedArtistReport: ArtistWorkReport;
  reportError$ = new Subject<boolean>();
  
  compareAvailable: boolean = false;
  report1: ArtistWorkReport;
  report2: ArtistWorkReport;
  comparisonReport: CompareRow[];

  displayedColumns: string[] = ['propertyName', 'value1', 'value2'];

  public barChartOptions: ChartOptions = {
    responsive: true,
  };
  public barChartLabels: Label[];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartPlugins = [];

  public barChartData: ChartDataSets[];

  constructor(
    private musicService: MusicService,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.getLastSearchedArtists();
  }

  searchArtists() {  
    this.clearSearches();
    this.loadingSearchResults = true;
    this.musicService.searchArtists(this.searchTerm).subscribe(
      (data) => {
        this.searchResults = data;
        let offsetLimit = this.searchResults.offset + this.searchResults.limit;
        this.searchResults.endRange = Math.min(offsetLimit, this.searchResults.totalResults);
        this.loadingSearchResults = false;
      },
      catchError((error) => {
        console.error('error loading the list of artists', error);
        this.searchError$.next(true);
        this.loadingSearchResults = false;
        return of();
      })
    );
  }

  getLastSearchedArtists() {
    this.loadingLastSearchedArtists = true;
    this.musicService.lastSearchedArtists().subscribe(
      (data) => {
        this.lastSearchedArtists = data;
        this.loadingLastSearchedArtists = false;
        this.compareAvailable = this.lastSearchedArtists.length >= 2;
      },
      catchError((error) => {
        console.error('error loading the list of artists', error);
        this.lastSearchError$.next(true);
        this.loadingLastSearchedArtists = false;
        return of();
      })
    );
  }

  getReport(id: string) {
    this.clearSearches();
    this.loadingReport = true;
    this.musicService.getArtistReport(id).subscribe(
      (data) => {
        this.selectedArtistReport = data;
        this.loadingReport = false;
        this.getLastSearchedArtists();
      },
      catchError((error) => {
        console.error('error loading the artist report for id ' + id, error);
        this.reportError$.next(true);
        this.loadingReport = false;
        return of();
      })
    );
  }

  viewSong(title: string, lyrics: string) {
    this.dialog.open(LyricsDialog, {
      autoFocus: false,
      maxHeight: '90vh',
      data: { title: title, lyrics: lyrics }
    });
  }

  compare() { 
    this.loadingReport = true;
    this.clearSearches();
    let report1Id = this.lastSearchedArtists[0].id;
    let report2Id = this.lastSearchedArtists[1].id;

    let report2$ = this.musicService.getArtistReport(report2Id);
    let report1$ = this.musicService.getArtistReport(report1Id);
    let releaseReport2$ = this.musicService.getReleasesReport(report2Id);
    let releaseReport1$ = this.musicService.getReleasesReport(report1Id);

    forkJoin([report2$, report1$, releaseReport2$, releaseReport1$]).subscribe(results => {
      this.report1 = results[1];
      this.report2 = results[0];

      let releaseReport2 = results[2];
      let releaseReport1 = results[3];

      let years = [
        ...releaseReport2.yearlyReleases.map(releaseYear => releaseYear.year),
        ...releaseReport1.yearlyReleases.map(releaseYear => releaseYear.year)
      ] // convert to object <== no need to sort: Properties are automatically sorted
      .reduce((acc, currentYear) => {
        if (acc[currentYear.toString()] !== null) { acc[currentYear.toString()] = null };
        return acc;
      }, {});

      const labels = Object.keys(years);
      this.barChartLabels = labels.map((label) => label.toString());

      this.barChartData = 
      [
        { label: this.report1.name, data: labels
            .map((year) => releaseReport1.yearlyReleases.find((x) => x.year == +year)?.releases || 0) },
        { label: this.report2.name, data: labels
          .map((year) => releaseReport2.yearlyReleases.find((x) => x.year == +year)?.releases || 0) },
      ];

      this.comparisonReport = [
        new CompareRow("Name", this.report1.name, this.report2.name, false, false),
        new CompareRow("Total Songs", `${this.report1.totalSongs}`, `${this.report2.totalSongs}`, this.report1.totalSongs > this.report2.totalSongs, this.report1.totalSongs < this.report2.totalSongs),
        new CompareRow("Lyrics Found", `${this.report1.songsConsidered}`, `${this.report2.songsConsidered}`, this.report1.songsConsidered > this.report2.songsConsidered, this.report1.songsConsidered < this.report2.songsConsidered),
        new CompareRow("Average Words", `${this.report1.averageWords}`, `${this.report2.averageWords}`, this.report1.averageWords > this.report2.averageWords, this.report1.averageWords < this.report2.averageWords),
        new CompareRow("Variance", `${this.report1.variance}`, `${this.report2.variance}`, this.report1.variance > this.report2.variance, this.report1.variance < this.report2.variance),
        new CompareRow("Std. Deviation", `${this.report1.standardDeviation}`, `${this.report2.standardDeviation}`, this.report1.standardDeviation > this.report2.standardDeviation, this.report1.standardDeviation < this.report2.standardDeviation),
        new CompareRow("Min Words", `${this.report1.minWords.wordCount}`, `${this.report2.minWords.wordCount}`, this.report1.minWords.wordCount > this.report2.minWords.wordCount, this.report1.minWords.wordCount < this.report2.minWords.wordCount),
        new CompareRow("Max Words", `${this.report1.maxWords.wordCount}`, `${this.report2.maxWords.wordCount}`, this.report1.maxWords.wordCount > this.report2.maxWords.wordCount, this.report1.maxWords.wordCount < this.report2.maxWords.wordCount),
      ]; 
  
      this.loadingReport = false;
    });
  }  

  clearSearches() {
    this.searchResults = null;
    this.comparisonReport = null;
    this.selectedArtistReport = null;
  }

  clearArtists() {
    this.searchTerm = "";
    this.clearSearches();
  }
}
