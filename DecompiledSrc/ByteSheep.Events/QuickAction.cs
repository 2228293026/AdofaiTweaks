namespace ByteSheep.Events;

public delegate void QuickAction();
public delegate void QuickAction<T>(T arg0);
public delegate void QuickAction<T, U>(T arg0, U arg1);
public delegate void QuickAction<T, U, V>(T arg0, U arg1, V arg2);
public delegate void QuickAction<T, U, V, W>(T arg0, U arg1, V arg2, W arg3);
