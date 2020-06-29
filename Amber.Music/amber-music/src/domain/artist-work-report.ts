import { ArtistWork } from "./artist-work";

export class ArtistWorkReport {
    artistId: string;

    name: string;

    totalSongs: number;

    songsConsidered: number;

    averageWords: number;

    variance: number;

    standardDeviation: number;

    minWords: ArtistWork;

    maxWords: ArtistWork;
}