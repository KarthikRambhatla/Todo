import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { TodoComponent } from './todo';
import { TodoService } from './todo.service';
import { of } from 'rxjs';

describe('TodoComponent', () => {
  let component: TodoComponent;
  let fixture: ComponentFixture<TodoComponent>;
  let service: jasmine.SpyObj<TodoService>;

  beforeEach(async () => {
    const mockService = jasmine.createSpyObj(
      'TodoService',
      ['getAll', 'add', 'delete', 'markDone']
    );

    await TestBed.configureTestingModule({
      imports: [TodoComponent],
      providers: [
        provideHttpClient(),
        { provide: TodoService, useValue: mockService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TodoComponent);
    component = fixture.componentInstance;
    service = TestBed.inject(TodoService) as jasmine.SpyObj<TodoService>;

    // Default stub
    service.getAll.and.returnValue(of([]));

    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should load todos on init', () => {
    const todos = [
      {
        id: '1',
        title: 'Test Item',
        createdAt: new Date(),
        updatedAt: new Date(),
        isDone: false
      }
    ];

    service.getAll.and.returnValue(of(todos));

    component.ngOnInit();
    fixture.detectChanges();

    expect(component.items().length).toBe(1);
    expect(component.items()[0].title).toBe('Test Item');
  });

  it('should call add() service when adding a todo', () => {
    service.add.and.returnValue(
      of({
        id: '2',
        title: 'New Task',
        createdAt: new Date(),
        updatedAt: new Date(),
        isDone: false
      })
    );

    component.newItem.set('New Task');
    component.add();

    expect(service.add).toHaveBeenCalledWith('New Task');
  });

  it('should call delete() service when deleting a todo', () => {
    service.delete.and.returnValue(of());

    component.delete('123');

    expect(service.delete).toHaveBeenCalledWith('123');
  });

  it('should call markDone() service when marking a todo done', () => {
    service.markDone.and.returnValue(
      of({
        id: '123',
        title: 'Test',
        createdAt: new Date(),
        updatedAt: new Date(),
        isDone: true
      })
    );

    component.markDone('123');

    expect(service.markDone).toHaveBeenCalledWith('123');
  });

  it('should add todo when pressing Enter key', () => {
    spyOn(component, 'add');

    const input: HTMLInputElement =
      fixture.nativeElement.querySelector('input');

    input.dispatchEvent(new KeyboardEvent('keyup', { key: 'Enter' }));

    expect(component.add).toHaveBeenCalled();
  });

  it('should correctly validate updated date', () => {
    const valid = {
      id: '1',
      title: 'Hello',
      createdAt: new Date('2025-01-01'),
      updatedAt: new Date('2025-01-02'),
      isDone: false
    };

    const invalid = {
      id: '2',
      title: 'Hello',
      createdAt: new Date(),
      updatedAt: new Date('0001-01-01'),
      isDone: false
    };

    expect(component.isValidUpdatedDate(valid)).toBeTrue();
    expect(component.isValidUpdatedDate(invalid)).toBeFalse();
  });
});
