import { Routes } from '@angular/router';
import { SubscribersListComponent } from './features/subscribers/subscribers-list.component';
import { SubscriberFormComponent } from './features/subscribers/subscriber-form.component';
import { SubscriberMailingListsComponent } from './features/subscribers/subscriber-mailing-lists.component';
import { MailingListsListComponent } from './features/mailing-lists/mailing-lists-list.component';
import { MailingListFormComponent } from './features/mailing-lists/mailing-list-form.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'subscribers' },
  { path: 'subscribers', component: SubscribersListComponent },
  { path: 'subscribers/new', component: SubscriberFormComponent },
  { path: 'subscribers/:id', component: SubscriberFormComponent },
  { path: 'subscribers/:id/mailing-lists', component: SubscriberMailingListsComponent },
  { path: 'mailing-lists', component: MailingListsListComponent },
  { path: 'mailing-lists/new', component: MailingListFormComponent },
  { path: 'mailing-lists/:id', component: MailingListFormComponent },
];
