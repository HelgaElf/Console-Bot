﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Bot
{
	public interface IToDoRepository
	{
		IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
		//Возвращает ToDoItem для UserId со статусом Active
		IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
		ToDoItem? Get(Guid id);
		void Add(ToDoItem item);
		void Update(ToDoItem item);
		void Delete(Guid id);
		//Проверяет есть ли задача с таким именем у пользователя
		bool ExistsByName(Guid userId, string name);
		//Возвращает количество активных задач у пользователя
		int CountActive(Guid userId);
        //возвращает все задачи пользователя, которые удовлетворяют предикату
		IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);
    }
}
