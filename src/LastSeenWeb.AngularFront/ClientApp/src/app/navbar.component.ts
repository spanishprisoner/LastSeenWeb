﻿import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppState } from './store/state/app.state';
import { accessToken } from './store/selectors/user.selectors';
import { AuthApiWrapperService } from './auth/auth-api-wrapper.service';
import { UpdatePopupService } from './lastseen-items/update-popup.service';
import { NotesPopupService } from './lastseen-items/notes-popup.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavBarComponent implements OnDestroy {
  private userSubscription: Subscription;
  loggedIn: boolean;

  constructor(private store: Store<AppState>, private authApiWrapperService: AuthApiWrapperService,
    private updatePopupService: UpdatePopupService, private notesPopupService: NotesPopupService) {
    this.userSubscription = this.store.select(accessToken).subscribe(e => this.loggedIn = !!e && e !== 'n/a');
  }

  ngOnDestroy(): void {
    this.userSubscription.unsubscribe();
  }

  notes(): void {
    this.notesPopupService.loadContent();
  }

  addNew(): void {
    this.updatePopupService.loadContent('');
  }

  logOut(): void {
    this.authApiWrapperService.logOut();
  }
}
