import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo',
  imports: [CommonModule, FormsModule],
  templateUrl: './todo.html',
  styleUrls: ['./todo.css']
})
export class Todo {
  items = signal<string[]>([]);
  newItem = signal('');

  add() {
    const value = this.newItem().trim();
    if (!value) return;
    this.items.update(list => [...list, value]);
    this.newItem.set('');
  }

  delete(i: number) {
    this.items.update(list => list.filter((_, idx) => idx !== i));
  }
}

