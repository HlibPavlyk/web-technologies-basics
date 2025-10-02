import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Shared contracts matching Lab6 API
export interface EntityIdModel { id: string; }
export interface GetItemsResponse<T> { count: number; items: T[]; }

// Subscribers
export interface SubscriberModel {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}
export interface UpdateSubscriberRequestModel {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

// Mailing lists
export interface MailingListModel {
  id: string;
  subject: string;
  content: string;
}
export interface UpdateMailingListModel {
  subject: string;
  content: string;
}

// Subscriber mailing lists (projection with LastSentDate)
export interface SubscriberMailingListModel extends MailingListModel {
  lastSentDate?: string | null; // ISO string from backend DateTime?
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api'; // proxied to Lab6 via proxy.conf.json

  // Subscribers
  getSubscribers(): Observable<GetItemsResponse<SubscriberModel>> {
    return this.http.get<GetItemsResponse<SubscriberModel>>(`${this.base}/subscribers`);
  }

  getSubscriber(id: string): Observable<SubscriberModel> {
    return this.http.get<SubscriberModel>(`${this.base}/subscribers/${id}`);
  }

  createSubscriber(body: UpdateSubscriberRequestModel): Observable<EntityIdModel> {
    return this.http.post<EntityIdModel>(`${this.base}/subscribers`, body);
  }

  updateSubscriber(id: string, body: UpdateSubscriberRequestModel): Observable<EntityIdModel> {
    return this.http.put<EntityIdModel>(`${this.base}/subscribers/${id}`, body);
  }

  deleteSubscriber(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/subscribers/${id}`);
  }

  // Mailing lists
  getMailingLists(): Observable<GetItemsResponse<MailingListModel>> {
    return this.http.get<GetItemsResponse<MailingListModel>>(`${this.base}/mailing-lists`);
  }

  getMailingList(id: string): Observable<MailingListModel> {
    return this.http.get<MailingListModel>(`${this.base}/mailing-lists/${id}`);
  }

  createMailingList(body: UpdateMailingListModel): Observable<EntityIdModel> {
    return this.http.post<EntityIdModel>(`${this.base}/mailing-lists`, body);
  }

  updateMailingList(id: string, body: UpdateMailingListModel): Observable<EntityIdModel> {
    return this.http.put<EntityIdModel>(`${this.base}/mailing-lists/${id}`, body);
  }

  deleteMailingList(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/mailing-lists/${id}`);
  }

  // Subscriber ↔ Mailing lists links
  getSubscriberMailingLists(id: string): Observable<GetItemsResponse<SubscriberMailingListModel>> {
    return this.http.get<GetItemsResponse<SubscriberMailingListModel>>(`${this.base}/subscribers/${id}/mailing-lists`);
    }

  addMailingListToSubscriber(id: string, mailingListId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/subscribers/${id}/mailing-lists/${mailingListId}`, {});
  }

  removeMailingListFromSubscriber(id: string, mailingListId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/subscribers/${id}/mailing-lists/${mailingListId}`);
  }
}
