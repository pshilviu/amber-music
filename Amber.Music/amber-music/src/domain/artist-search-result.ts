import { ArtistSearch } from "./artist-search";

export class ArtistSearchResult {
    totalResults: number;

    limit: number;

    offset: number;

    endRange: number;

    results: ArtistSearch[]
}