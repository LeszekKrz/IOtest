import { Component,} from '@angular/core';
import { SearchResultsDTO } from 'src/app/core/models/search-results-dto';
import { SearchService } from 'src/app/core/services/search.service';
import { SortingTypes } from 'src/app/core/models/enums/sorting-types';
import { SortingDirections } from 'src/app/core/models/enums/sorting-directions';
import { Router } from '@angular/router';
import { VideoMetadataDto } from 'src/app/core/models/video-metadata-dto';
import { getTimeAgo } from 'src/app/core/functions/get-time-ago';
import { finalize, Observable, of, Subscription, switchMap } from 'rxjs';

interface SortTypeOption {
  label: string;
  value: SortingTypes;
}

interface SortDirectionOption {
  label: string;
  value: SortingDirections;
}

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {
  isProgressSpinnerVisible: boolean = false;
  subscriptions: Subscription[] = [];
  query: string = '';
  beginDate?: Date;
  endDate?: Date;
  searchResults!: SearchResultsDTO;
  sortingTypes: SortTypeOption[];
  sortingDirections: SortDirectionOption[];
  sortingType!: SortTypeOption;
  sortingDirection!: SortDirectionOption;

  constructor(
    private searchService: SearchService,
    private router: Router,
    ) {
    this.sortingTypes = [
      { label: 'Alphabetical', value: SortingTypes.Alphabetical },
      { label: 'Popularity', value: SortingTypes.Popularity },
      { label: 'Publish date', value: SortingTypes.PublishDate }
    ];

    this.sortingDirections = [
      { label: 'Ascending', value: SortingDirections.Asceding },
      { label: 'Descending', value: SortingDirections.Descending }
    ]

    this.sortingDirection = this.sortingDirections[0];
    this.sortingType = this.sortingTypes[0];

    let routerState = this.router.getCurrentNavigation()?.extras.state;
    if (routerState != undefined) {
      this.query = routerState['query'];
      this.performSearch();
    }
  }

  onSearchButtonClick() {
    if (this.areDatesValid()) {
      this.performSearch();
    }
  }

  areDatesValid() {
    if (this.beginDate && this.endDate && this.beginDate > this.endDate) {
      return false;
    }
    return true;
  }

  isQueryEmpty() {
    return (this.query.length === 0);
  }

  performSearch() {
    const search$ = this.searchService.getSearchResults(this.query, this.sortingType.value, this.sortingDirection.value,
      this.beginDate, this.endDate);
    this.subscriptions.push(this.doWithLoading(search$).subscribe(result => this.searchResults = result));

    }

  searchResultsNone() {
    return this.searchResults !== undefined && this.searchResults.users.length === 0 && this.searchResults.videos.length === 0; 
  }

  doWithLoading(observable$: Observable<any>): Observable<any> {
    return of(this.isProgressSpinnerVisible = true).pipe(
      switchMap(() => observable$),
      finalize(() => this.isProgressSpinnerVisible = false)
    );
  }

  goToUserProfile(id: string): void {
    this.router.navigate(['creator/' + id]);
  }

  goToVideo(id: string): void {
    this.router.navigate(['videos/' +id]);
  }

  public getTimeAgo(video: VideoMetadataDto): string {
    return getTimeAgo(video.uploadDate);
  }
}
