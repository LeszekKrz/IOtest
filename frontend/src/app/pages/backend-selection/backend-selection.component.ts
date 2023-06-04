import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-backend-selection',
  templateUrl: './backend-selection.component.html',
  styleUrls: ['./backend-selection.component.scss']
})
export class BackendSelectionComponent {
  groups = [
    {name: 'Group 10', number: '10'},
    {name: 'Group 11', number: '11'},
    {name: 'Group 12', number: '12'},
    {name: 'Group 13', number: '13'}
  ];
  selectedGroup = this.groups[this.getSelectedGroup()];

  onButtonClick(group: any) {
    localStorage.setItem('api', group.number);
  }

  private getSelectedGroup(): number {
    let api = localStorage.getItem('api');
    if (api === '10') {
      return 0;
    }
    else if (api === '11') {
      return 1;
    }
    else if (api === '12') {
      return 2;
    }
    else if (api === '13') {
      return 3;
    }

    return 1;
  }
}
