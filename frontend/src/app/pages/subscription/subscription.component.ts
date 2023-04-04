import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SubscriptionDto } from 'src/app/core/models/subscription-dto';
import { SubbscriptionListDto } from 'src/app/core/models/subscription-list-dto';
import { SubscriptionService } from 'src/app/core/services/subscription.service';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent {
  id!: string;
  subscriptions!: SubscriptionDto[];

  constructor(
    private route: ActivatedRoute, 
    private subscriptionService: SubscriptionService,
    private router: Router,
    ) {
    this.id = this.route.snapshot.paramMap.get('id')!;
    subscriptionService.getUserVideos(this.id).subscribe(subscriptions => this.subscriptions = subscriptions.subscriptions);
  }

  public goToUserProfile(id: string): void {
    this.router.navigate(['creator/' + id]);
  }
}
