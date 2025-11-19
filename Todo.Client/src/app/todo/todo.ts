import { Component, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TodoService } from './todo.service';
import { TodoItem } from './todo.model';

@Component({
  selector: 'app-todo',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './todo.html',
})
export class TodoComponent implements OnInit {

  items = signal<TodoItem[]>([]);
  newItem = signal<string>('');

  constructor(private api: TodoService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getAll().subscribe(data => this.items.set(data));
  }

  add() {
    const title = this.newItem().trim();
    if (!title) return;

    this.api.add(title).subscribe(saved => {
      this.items.set([...this.items(), saved]);
      this.newItem.set('');
    });
  }

  delete(id: string) {
    this.api.delete(id).subscribe(() => {
      this.items.set(this.items().filter(x => x.id !== id));
    });
  }

  markDone(id: string) {
    this.api.markDone(id).subscribe(updatedItem => {
      this.items.set(
        this.items().map(x => x.id === updatedItem.id ? updatedItem : x)
      );
    });
  }

  isValidUpdatedDate(item: TodoItem): boolean {
  if (!item.updatedAt) return false;

  const minValid = new Date(1900, 0, 1);   // Jan 1, 1900
  return new Date(item.updatedAt) > minValid &&
         item.updatedAt !== item.createdAt;
}}
