using System;
using AGRO_GRAMM;



using System;



public class Parser {
	public const int _EOF = 0;
	public const int _id = 1;
	public const int _cte_I = 2;
	public const int _cte_F = 3;
	public const int _ctr_Str = 4;
	public const int _cbl = 5;
	public const int _cbr = 6;
	public const int _bl = 7;
	public const int _br = 8;
	public const int _pl = 9;
	public const int _pr = 10;
	public const int _comma = 11;
	public const int _semicolon = 12;
	public const int _add = 13;
	public const int _sub = 14;
	public const int _mul = 15;
	public const int _div = 16;
	public const int _equal = 17;
	public const int _dot = 18;
	public const int _sadd = 19;
	public const int _ssub = 20;
	public const int _sdiv = 21;
	public const int _smul = 22;
	public const int _increment = 23;
	public const int _decrement = 24;
	public const int _colon = 25;
	public const int _less = 26;
	public const int _greater = 27;
	public const int _lesseq = 28;
	public const int _greatereq = 29;
	public const int _equaleq = 30;
	public const int _different = 31;
	public const int maxT = 48;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int // types
	  undef = 0, t_int = 1, t_float = 2, t_char = 3, t_void = 4 ,t_obj = 5;

const int // object kinds
	  var = 0, func = 1;

SymbolTable   sTable;

/*--------------------------------------------------------------------------*/    

bool IsFunctionCall(){
    scanner.ResetPeek();
    Token x = la; 
    while (x.kind == _id ) 
        x = scanner.Peek();
    return x.kind == _pl;
}

bool IsMethodCall() { 
    scanner.ResetPeek();
    Token x = la; 
    while (x.kind == _id || x.kind == _dot) 
        x = scanner.Peek();
    return x.kind == _pl;
} 

bool IsTypeFunction() {
    scanner.ResetPeek();
    Token next = scanner.Peek();
    next = scanner.Peek();
    return next.kind == _pl;
}

bool IsDecVars(){
    scanner.ResetPeek();
    Token x = scanner.Peek();
    while (x.kind == _id || x.kind == _comma) 
        x = scanner.Peek();
    return x.kind == _semicolon;
}



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void PROGRAM() {
		sTable = new SymbolTable();
		
		while (StartOf(1)) {
			DECLARATION();
		}
		MAIN();
	}

	void DECLARATION() {
		if (IsTypeFunction() ) {
			DEC_FUNC();
		} else if (StartOf(2)) {
			DEC_VARS();
		} else SynErr(49);
	}

	void MAIN() {
		sTable = sTable.newChildSymbolTable(); 
		Expect(35);
		Expect(5);
		if (IsDecVars() ) {
			DEC_VARS();
		} else if (StartOf(3)) {
			STATUTE();
		} else SynErr(50);
		while (StartOf(4)) {
			if (IsDecVars() ) {
				DEC_VARS();
			} else {
				STATUTE();
			}
		}
		Expect(6);
		sTable = sTable.parentSymbolTable; 
	}

	void DEC_FUNC() {
		string name; int type; 
		TYPE_FUNC(out type );
		IDENT(out name );
		sTable.putSymbol(name, type, func);
		       sTable = sTable.newChildSymbolTable(); 
		Expect(9);
		if (la.kind == 32 || la.kind == 33 || la.kind == 34) {
			PARAMS_FUNC();
		}
		Expect(10);
		Expect(5);
		if (StartOf(4)) {
			if (IsDecVars() ) {
				DEC_VARS();
			} else {
				STATUTE();
			}
			while (StartOf(4)) {
				if (IsDecVars() ) {
					DEC_VARS();
				} else {
					STATUTE();
				}
			}
		}
		if (la.kind == 37) {
			RETURN();
		}
		Expect(6);
		sTable = sTable.parentSymbolTable; 
	}

	void DEC_VARS() {
		string name; int type; string className; 
		if (la.kind == 1) {
			IDENT(out className );
			IDENT(out name );
			sTable.putSymbol(name, t_obj, var); 
			while (la.kind == 11) {
				Get();
				IDENT(out name );
				sTable.putSymbol(name, t_obj, var); 
			}
			Expect(12);
		} else if (la.kind == 32 || la.kind == 33 || la.kind == 34) {
			SIMPLE_TYPE(out type );
			IDENT(out name );
			sTable.putSymbol(name, type, var); 
			if (la.kind == 7) {
				Get();
				Expect(2);
				Expect(8);
				if (la.kind == 7) {
					Get();
					Expect(2);
					Expect(8);
				}
			}
			while (la.kind == 11) {
				Get();
				IDENT(out name );
				sTable.putSymbol(name, type, var); 
				if (la.kind == 7) {
					Get();
					Expect(2);
					Expect(8);
					if (la.kind == 7) {
						Get();
						Expect(2);
						Expect(8);
					}
				}
			}
			Expect(12);
		} else SynErr(51);
	}

	void IDENT(out string name ) {
		Expect(1);
		name = t.val; 
	}

	void SIMPLE_TYPE(out int type ) {
		type = undef; 
		if (la.kind == 32) {
			Get();
			type = t_int; 
		} else if (la.kind == 33) {
			Get();
			type = t_float; 
		} else if (la.kind == 34) {
			Get();
			type = t_char; 
		} else SynErr(52);
	}

	void TYPE_FUNC(out int type ) {
		type = undef; 
		if (la.kind == 32) {
			Get();
			type = t_int; 
		} else if (la.kind == 33) {
			Get();
			type = t_float; 
		} else if (la.kind == 34) {
			Get();
			type = t_char; 
		} else if (la.kind == 36) {
			Get();
			type = t_void; 
		} else SynErr(53);
	}

	void PARAMS_FUNC() {
		string name; int type; 
		SIMPLE_TYPE(out type );
		IDENT(out name );
		sTable.putSymbol(name, type, var); 
		while (la.kind == 11) {
			Get();
			SIMPLE_TYPE(out type );
			IDENT(out name );
			sTable.putSymbol(name, type, var); 
		}
	}

	void STATUTE() {
		if (la.kind == 38) {
			INPUT();
		} else if (la.kind == 39) {
			PRINT();
		} else if (IsFunctionCall() ) {
			FUNC_CALL();
		} else if (la.kind == 40) {
			CONDITIONAL();
		} else if (la.kind == 42) {
			WHILE();
		} else if (la.kind == 43) {
			FOR();
		} else if (la.kind == 1) {
			ASSIGN();
		} else SynErr(54);
	}

	void RETURN() {
		Expect(37);
		EXP();
		Expect(12);
	}

	void INPUT() {
		Expect(38);
		Expect(9);
		VARIABLE_ASSIGN();
		Expect(10);
		Expect(12);
	}

	void PRINT() {
		Expect(39);
		Expect(9);
		EXP();
		while (la.kind == 11) {
			Get();
			EXP();
		}
		Expect(10);
		Expect(12);
	}

	void FUNC_CALL() {
		string name; 
		IDENT(out name );
		Expect(9);
		if (StartOf(5)) {
			EXP();
			while (la.kind == 11) {
				Get();
				EXP();
			}
		}
		Expect(10);
		Expect(12);
	}

	void CONDITIONAL() {
		Expect(40);
		Expect(9);
		HYPER_EXP();
		Expect(10);
		BLOCK();
		if (la.kind == 41) {
			Get();
			BLOCK();
		}
	}

	void WHILE() {
		Expect(42);
		Expect(9);
		HYPER_EXP();
		Expect(10);
		BLOCK();
	}

	void FOR() {
		Expect(43);
		Expect(9);
		ASSIGN();
		Expect(12);
		HYPER_EXP();
		Expect(12);
		ASSIGN();
		Expect(10);
		BLOCK();
	}

	void ASSIGN() {
		VARIABLE_ASSIGN();
		if (StartOf(6)) {
			if (StartOf(7)) {
				SHORT_ASSIGN();
			} else {
				Get();
			}
			EXP();
		} else if (la.kind == 23 || la.kind == 24) {
			STEP();
		} else SynErr(55);
		Expect(12);
	}

	void EXP() {
		TERM();
		while (la.kind == 13 || la.kind == 14) {
			if (la.kind == 13) {
				Get();
			} else {
				Get();
			}
			TERM();
		}
	}

	void TERM() {
		FACT();
		while (la.kind == 15 || la.kind == 16) {
			if (la.kind == 15) {
				Get();
			} else {
				Get();
			}
			FACT();
		}
	}

	void VARIABLE_ASSIGN() {
		string name; 
		IDENT(out name );
		if (la.kind == 7) {
			Get();
			EXP();
			Expect(8);
			if (la.kind == 7) {
				Get();
				EXP();
				Expect(8);
			}
		}
	}

	void SHORT_ASSIGN() {
		if (la.kind == 19) {
			Get();
		} else if (la.kind == 20) {
			Get();
		} else if (la.kind == 22) {
			Get();
		} else if (la.kind == 21) {
			Get();
		} else SynErr(56);
	}

	void STEP() {
		if (la.kind == 23) {
			Get();
		} else if (la.kind == 24) {
			Get();
		} else SynErr(57);
	}

	void HYPER_EXP() {
		SUPER_EXP();
		while (StartOf(8)) {
			REL_EXP();
			SUPER_EXP();
		}
	}

	void BLOCK() {
		Expect(5);
		while (StartOf(3)) {
			STATUTE();
		}
		Expect(6);
	}

	void FACT() {
		if (la.kind == 9) {
			Get();
			EXP();
			Expect(10);
		} else if (StartOf(9)) {
			if (la.kind == 13 || la.kind == 14) {
				if (la.kind == 13) {
					Get();
				} else {
					Get();
				}
			}
			if (la.kind == 2) {
				Get();
			} else if (la.kind == 3) {
				Get();
			} else SynErr(58);
		} else if (la.kind == 4) {
			Get();
		} else if (la.kind == 1) {
			VARIABLE_FACT();
		} else SynErr(59);
	}

	void VARIABLE_FACT() {
		string name; 
		IDENT(out name );
		if (la.kind == 7 || la.kind == 9) {
			if (la.kind == 7) {
				Get();
				EXP();
				Expect(8);
				if (la.kind == 7) {
					Get();
					EXP();
					Expect(8);
				}
			} else {
				Get();
				if (StartOf(5)) {
					EXP();
					while (la.kind == 11) {
						Get();
						EXP();
					}
				}
				Expect(10);
			}
		}
	}

	void SUPER_EXP() {
		EXP();
		if (StartOf(10)) {
			REL_OP();
			EXP();
		}
	}

	void REL_EXP() {
		if (la.kind == 44) {
			Get();
		} else if (la.kind == 45) {
			Get();
		} else if (la.kind == 46) {
			Get();
		} else if (la.kind == 47) {
			Get();
		} else SynErr(60);
	}

	void REL_OP() {
		if (la.kind == 26 || la.kind == 27) {
			if (la.kind == 27) {
				Get();
			} else {
				Get();
			}
		} else if (la.kind == 28 || la.kind == 29) {
			if (la.kind == 29) {
				Get();
			} else {
				Get();
			}
		} else if (la.kind == 30 || la.kind == 31) {
			if (la.kind == 30) {
				Get();
			} else {
				Get();
			}
		} else SynErr(61);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		PROGRAM();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_T,_T, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_x, _x,_x,_T,_T, _T,_x,_T,_T, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x},
		{_x,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "id expected"; break;
			case 2: s = "cte_I expected"; break;
			case 3: s = "cte_F expected"; break;
			case 4: s = "ctr_Str expected"; break;
			case 5: s = "cbl expected"; break;
			case 6: s = "cbr expected"; break;
			case 7: s = "bl expected"; break;
			case 8: s = "br expected"; break;
			case 9: s = "pl expected"; break;
			case 10: s = "pr expected"; break;
			case 11: s = "comma expected"; break;
			case 12: s = "semicolon expected"; break;
			case 13: s = "add expected"; break;
			case 14: s = "sub expected"; break;
			case 15: s = "mul expected"; break;
			case 16: s = "div expected"; break;
			case 17: s = "equal expected"; break;
			case 18: s = "dot expected"; break;
			case 19: s = "sadd expected"; break;
			case 20: s = "ssub expected"; break;
			case 21: s = "sdiv expected"; break;
			case 22: s = "smul expected"; break;
			case 23: s = "increment expected"; break;
			case 24: s = "decrement expected"; break;
			case 25: s = "colon expected"; break;
			case 26: s = "less expected"; break;
			case 27: s = "greater expected"; break;
			case 28: s = "lesseq expected"; break;
			case 29: s = "greatereq expected"; break;
			case 30: s = "equaleq expected"; break;
			case 31: s = "different expected"; break;
			case 32: s = "\"int\" expected"; break;
			case 33: s = "\"float\" expected"; break;
			case 34: s = "\"char\" expected"; break;
			case 35: s = "\"main\" expected"; break;
			case 36: s = "\"void\" expected"; break;
			case 37: s = "\"return\" expected"; break;
			case 38: s = "\"input\" expected"; break;
			case 39: s = "\"print\" expected"; break;
			case 40: s = "\"if\" expected"; break;
			case 41: s = "\"else\" expected"; break;
			case 42: s = "\"while\" expected"; break;
			case 43: s = "\"for\" expected"; break;
			case 44: s = "\"and\" expected"; break;
			case 45: s = "\"&&\" expected"; break;
			case 46: s = "\"or\" expected"; break;
			case 47: s = "\"||\" expected"; break;
			case 48: s = "??? expected"; break;
			case 49: s = "invalid DECLARATION"; break;
			case 50: s = "invalid MAIN"; break;
			case 51: s = "invalid DEC_VARS"; break;
			case 52: s = "invalid SIMPLE_TYPE"; break;
			case 53: s = "invalid TYPE_FUNC"; break;
			case 54: s = "invalid STATUTE"; break;
			case 55: s = "invalid ASSIGN"; break;
			case 56: s = "invalid SHORT_ASSIGN"; break;
			case 57: s = "invalid STEP"; break;
			case 58: s = "invalid FACT"; break;
			case 59: s = "invalid FACT"; break;
			case 60: s = "invalid REL_EXP"; break;
			case 61: s = "invalid REL_OP"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
