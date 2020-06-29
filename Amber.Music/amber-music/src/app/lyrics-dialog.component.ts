import { Component, Inject } from '@angular/core';
import { LyricsDialogData } from '../domain/lyrics-dialog-data'
import { MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
    selector: 'lyrics-dialog',
    templateUrl: 'lyrics-dialog.component.html',
    styles: [`
      .lyrics-dialog {
        white-space: pre-line !important;
      }
    `]
  })
  
  export class LyricsDialog {
    constructor(@Inject(MAT_DIALOG_DATA) public data: LyricsDialogData) {}
  }