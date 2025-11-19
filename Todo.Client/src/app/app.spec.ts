import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { TodoService } from './todo/todo.service';
import { of } from 'rxjs';
import { TodoComponent } from './todo/todo';

describe('App', () => {
  let todoServiceSpy: jasmine.SpyObj<TodoService>;

  beforeEach(async () => {
    todoServiceSpy = jasmine.createSpyObj('TodoService', ['getAll']);
    todoServiceSpy.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [
        App,
        TodoComponent,
      ],
      providers: [
        { provide: TodoService, useValue: todoServiceSpy }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Todo List');
  });
});
