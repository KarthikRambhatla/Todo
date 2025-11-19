import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TodoItem } from './todo.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private baseUrl = environment.apiUrl + '/api/Todo'; 

  constructor(private http: HttpClient) {}

  getAll(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.baseUrl, { withCredentials: true });
  }

  getById(id: string): Observable<TodoItem> {
    return this.http.get<TodoItem>(`${this.baseUrl}/${id}`, { withCredentials: true });
  }

  add(title: string): Observable<TodoItem> {
    const body = { title };
    return this.http.post<TodoItem>(this.baseUrl, body, { withCredentials: true });
  }

  delete(id: string): Observable<void> {
  return this.http.delete<void>(`${this.baseUrl}/${id}`, { withCredentials: true });
}

markDone(id: string): Observable<TodoItem> {
  return this.http.patch<TodoItem>(`${this.baseUrl}/${id}/done`, {}, { withCredentials: true });
}
}
