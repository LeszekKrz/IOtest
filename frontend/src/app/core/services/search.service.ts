import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { SearchResultsDTO } from "../models/search-results-dto";
import { SortingDirections } from '../models/enums/sorting-directions';
import { SortingTypes } from '../models/enums/sorting-types';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";


@Injectable({
  providedIn: 'root'
})
export class SearchService {

  private readonly searchPageWebAPIUrl: string = `${environment.webApiUrl}`;

  constructor(private httpClient: HttpClient) {}

  getSearchResults(query: string, sortingType: SortingTypes, sortingDirection: SortingDirections,
    dateBegin?: Date, dateEnd?: Date): Observable<SearchResultsDTO> {    
    
    let params = new HttpParams()
      .set('query', query).set('sortingCriterion', sortingType).set('sortingType', sortingDirection);

    if (dateBegin != null)
      params = params.set('beginDate', dateBegin.toDateString());
    if (dateEnd != null)
      params = params.set('endDate', dateEnd.toDateString());

      const httpOptions = {
        params: params,
        headers: getHttpOptionsWithAuthenticationHeader().headers
      };
    
    return this.httpClient.get<SearchResultsDTO>(`${this.searchPageWebAPIUrl}/search`, httpOptions);
  }
}
