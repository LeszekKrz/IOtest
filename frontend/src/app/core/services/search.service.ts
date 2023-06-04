import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { SearchResultsDTO } from "../models/search-results-dto";
import { SortingDirections } from '../models/enums/sorting-directions';
import { SortingTypes } from '../models/enums/sorting-types';
import { getHttpOptionsWithAuthenticationHeader } from "../functions/get-http-options-with-authorization-header";
import { getApiUrl } from '../functions/get-api-url';


@Injectable({
  providedIn: 'root'
})
export class SearchService {
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
    
    return this.httpClient.get<SearchResultsDTO>(`${getApiUrl()}/search`, httpOptions);
  }
}
