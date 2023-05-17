import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Comment } from './models/comment';
import { FormControl, Validators } from '@angular/forms';
import { MenuItem } from 'primeng/api';
import { CommentsService } from './services/comments.service';
import { Subscription, tap } from 'rxjs';
import { CommentsDTO } from './models/comments-dto';

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.scss']
})
export class CommentsComponent implements OnInit, OnDestroy {
  @Input() videoId!: string;
  comments!: Comment[];
  newComment = new FormControl('', Validators.required);
  subscriptions: Subscription[] = [];
  isProgressSpinnerVisible = false;

  constructor(private commentService: CommentsService) { }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
  }
  
  ngOnInit(): void {
    this.getAllComments();
  }

  getAllComments(): void {
    const getAllComments$ = this.commentService.getAllComments(this.videoId).pipe(
      tap((commentsDTO: CommentsDTO) => {
        this.comments = commentsDTO.comments.map(comment => ({
          id: comment.id,
          authorId: comment.authorId,
          content: comment.content,
          avatarImage: comment.avatarImage,
          nickname: comment.nickname,
          hasResponses: comment.hasResponses,
          responses: null,
          isResponsesVisible: false,
          isResponseGettingAdded: false,
          responseFormControl: new FormControl('', Validators.required),
        }))
      }),
    );

    this.subscriptions.push(getAllComments$.subscribe());
  }

  getAllCommentResponses(comment: Comment): void {
    const getAllCommentResponses$ = this.commentService.getAllCommentResponses(comment.id).pipe(
      tap((commentsDTO: CommentsDTO) => {
        comment.responses = commentsDTO.comments.map(response => ({
          id: response.id,
          authorId: response.authorId,
          content: response.content,
          avatarImage: response.avatarImage,
          nickname: response.nickname,
        }))
      }),
    );

    this.subscriptions.push(getAllCommentResponses$.subscribe());
  }

  isEmptyAndTouchOrDirty(formControl: FormControl): boolean {
    return formControl.invalid && (formControl.touched || formControl.dirty);
  }

  isTouchOrDirty(formControl: FormControl): boolean {
    return formControl.touched || formControl.dirty;
  }

  handleOnCancelCommentClick(formControl: FormControl): void {
    formControl.reset();
  }

  handleOnAddCommentClick(formControl: FormControl): void {
    if (formControl.invalid) {
      formControl.markAsDirty();
      return;
    }

    const addComment$ = this.commentService.addComment(formControl.value, this.videoId).pipe(
      tap(() => {
        this.getAllComments();
        formControl.reset();
      }),
    );
    this.subscriptions.push(addComment$.subscribe());
  }

  isEmpty(formControl: FormControl): boolean {
    return formControl.value === '';
  }

  deleteComment(commentIndex: number): void {
    const deleteComment$ = this.commentService.deleteComment(this.comments[commentIndex].id).pipe(
      tap(() => {
        this.comments.splice(commentIndex, 1);
      }),
    );

    this.subscriptions.push(deleteComment$.subscribe());
  }

  reportComment(commentIndex: number): void {
    // REPORT COMMENT LOGIC HERE
  }

  handleOnHideResponsesClick(comment: Comment): void {
    comment.isResponsesVisible = false;
  }

  handleOnShowResponsesClick(comment: Comment): void {
    this.getAllCommentResponses(comment);
    comment.isResponsesVisible = true;
  }

  deleteCommentResponse(comment: Comment, responseIndex: number): void {
    const deleteCommentResponse$ = this.commentService.deleteCommentResponse(comment.responses![responseIndex].id).pipe(
      tap(() => {
        this.getAllCommentResponses(comment);
      }),
    );

    this.subscriptions.push(deleteCommentResponse$.subscribe());
  }

  reportCommentResponse(comment: Comment, responseIndex: number): void {
    // REPORT COMMENT RESPONSE LOGIC HERE
  }

  handleOnAddResponseClick(comment: Comment): void {
    comment.isResponseGettingAdded = true;
  }

  handleOnCancelCommentResponseClick(comment: Comment): void {
    comment.isResponseGettingAdded = false;
    comment.responseFormControl.reset();
  }

  handleOnAddCommentResponseClick(comment: Comment): void {
    if (comment.responseFormControl.invalid) {
      comment.responseFormControl.markAsDirty();
      return;
    }

    const addCommentResponse$ = this.commentService.addCommentResponse(comment.responseFormControl.value, comment.id).pipe(
      tap(() => {
        if (comment.isResponsesVisible) {
          this.getAllCommentResponses(comment);
        }
        comment.responseFormControl.reset();
        comment.isResponseGettingAdded = false;
        comment.hasResponses = true;
      }),
    );
    this.subscriptions.push(addCommentResponse$.subscribe());
  }

  getCommentMenuModel(commentIndex: number): MenuItem[] {
    return [
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.deleteComment(commentIndex),
      },
      {
        label: 'Report',
        icon: 'pi pi-flag',
        command: () => this.reportComment(commentIndex),
      },
    ];
  }

  getCommentResponseMenuModel(comment: Comment, responseIndex: number): MenuItem[] {
    return [
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.deleteCommentResponse(comment, responseIndex),
      },
      {
        label: 'Report',
        icon: 'pi pi-flag',
        command: () => this.reportCommentResponse(comment, responseIndex),
      },
    ];
  }
}
