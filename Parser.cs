using System;
using Newtonsoft.Json;
using AGRO_GRAMM;
using System.Collections.Generic;




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
	public const int _exponent = 16;
	public const int _div = 17;
	public const int _intdiv = 18;
	public const int _module = 19;
	public const int _equal = 20;
	public const int _dot = 21;
	public const int _sadd = 22;
	public const int _ssub = 23;
	public const int _sdiv = 24;
	public const int _smul = 25;
	public const int _increment = 26;
	public const int _decrement = 27;
	public const int _colon = 28;
	public const int _less = 29;
	public const int _greater = 30;
	public const int _lesseq = 31;
	public const int _greatereq = 32;
	public const int _equaleq = 33;
	public const int _different = 34;
	public const int _and = 35;
	public const int _or = 36;
	public const int maxT = 52;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

const int // types
	  invalid = Int32.MaxValue, undef = 0, t_int = 1, t_float = 2, t_char = 3, t_void = 4 ,t_obj = 5, t_string = 6;

const int // object kinds
	  var = 0, func = 1, temporal = 2, pointer = 3, constant = 4, array = 5;


int[] TERM_OPERATORS = { _mul, _div, _exponent, _intdiv, _module};
int[] EXP_OPERATORS = { _add, _sub };
int[] RELEXP_OPERATORS = { _and, _or };
int[] RELOP_OPERATORS = { _greater, _less, _greatereq, _lesseq, _equaleq, _different };

//extra tokens for Quads
public const int _print=66;
public const int _input=67;
public const int _goto=68;
public const int _gotoFalse=69;
public const int _gotoTrue=70;

Dictionary<int, string> operandInts = JsonConvert.DeserializeObject<Dictionary<int, string>>(@$"{{
				{_add}:'+',
				{_sub}:'-',
				{_div}:'/',
				{_exponent}:'**',
                {_intdiv}:'//',
				{_mul}:'*',
                {_module}:'%',
				{_sadd}:'+=',
				{_ssub}:'-=',
				{_sdiv}:'/=',
				{_smul}:'*=',
				{_increment}: '++',
				{_decrement}: '--',
				{_less}:'<',
				{_lesseq}:'<=',
				{_greater}:'>',
				{_greatereq}:'>=',
				{_equaleq}:'==',
				{_equal}:'=',
				{_different}:'!=',
				{_and}:'&&',
				{_and}:'and',
				{_or}:'or',
				{_or}:'||',
                {_print}: 'print',
                {_input}: 'input',
                {_goto}: 'goto',
                {_gotoFalse}: 'gotoFalse',
                {_gotoTrue}: 'gotoTrue'
				}}");

Dictionary<int, string> typesInts = JsonConvert.DeserializeObject<Dictionary<int, string>>(@$"{{
				{invalid}: 'INVALID',
                {undef}:'UNDEFINED',
				{t_int}:'INT',
				{t_float}:'FLOAT',
				{t_char}:'CHAR',
				{t_void}:'VOID',
				{t_obj}:'OBJ',
				{t_string}:'STRING'
				}}");

public SymbolTable   sTable;
public Dictionary<string, Function> dirFunc   = new Dictionary<string, Function>();
public Dictionary<string, Classes> dirClasses = new Dictionary<string, Classes>();

Stack<String> stackOperand = new Stack<String>();
Stack<int>   stackOperator = new Stack<int>();
Stack<int>      stackTypes = new Stack<int>();
Stack<int>      stackJumps = new Stack<int>();

int tempCont = 0;

public List<Actions> program = new List<Actions>();

void pushToOperandStack(string id, SymbolTable st){
    int typeId = invalid;
    if(validateOper(id,st)){
        // In order to push to the stack, we need to know the type of the id
        typeId = st.getType(id);
    }
    // Push the id
    stackOperand.Push(id);
    // Push the type
    stackTypes.Push(typeId);

}

string createTemp(int type, SymbolTable st){
    string tempName;
    tempName = "_t" + tempCont;
    tempCont+=1;
    st.putSymbol(tempName, type, temporal, 1);
    return tempName;
}

string createTempInt(int tempp, SymbolTable st){
    string tempName;
    tempName = "_t" + tempCont;
    tempCont+=1;
    st.putSymbol(tempName, t_int, temporal, 1);
    return tempName;
}

string createConstInt(int value, SymbolTable st){
    string constName;
    constName = "_" + value;
    st.putConstantInt(constName, t_int, constant, value);
    return constName;
}

string createConstFloat(float value, SymbolTable st){
    string constName;
    constName = "_" + value;
    st.putConstantFloat(constName, t_float, constant, value);
    return constName;
}

string createConstString(string value, SymbolTable st){
    string constName;
    constName = "_" + value;
    st.putConstantString(constName, t_string, constant, value);
    return constName;
}

string createTempFloat(float tempp, SymbolTable st){
    string tempName;
    tempName = "_t" + tempCont;
    tempCont+=1;
    st.putSymbol(tempName, t_float, temporal, 1);
    return tempName;
}

bool validateOper(string oper, SymbolTable st){
    return (st.getSymbol(oper) != null);
}

void checkReturn(SymbolTable st, string leftOper, string rightOper) {
    Cuadruple quad = new Cuadruple(_equal, leftOper, rightOper, leftOper, st, operandInts);

    // Check if cube operator is valid for these operands
    if (quad.typeOut == invalid)
    {
        SemErr("Return type mismatch: Expected <" + typesInts[st.getType(leftOper)] + ">. Found <" + typesInts[st.getType(rightOper)] + ">");
    }
    //program.Add(quad);
}

void checkAssign(SymbolTable st) {
    string leftOper;
    string rightOper;
    int leftType;
    int rightType;
    int operat;

    if(stackOperator.Count > 0) {
        // Get the data to create the quad
        rightType = stackTypes.Pop();
        rightOper = stackOperand.Pop();
        if(rightType == invalid){
            SemErr("Invalid assignment: " + rightOper + " not declared.");
            return;
        }

        if (stackOperand.Count > 0) {
            leftType = stackTypes.Pop();
            leftOper = stackOperand.Pop();
            if(leftType == invalid){
                SemErr("Invalid assignment: " + leftOper + " not declared.");
                return;
            }
        }
        else {
            leftType = rightType;
            leftOper = rightOper;
        }
        
        operat = stackOperator.Pop();
        
        Assign assign = new Assign(operat, rightOper, leftOper, st, operandInts);

        // Check if cube operator is valid for these operands
        if (assign.typeOut == invalid)
        {
            SemErr("Invalid assignment: " + typesInts[leftType] + " " + operandInts[operat] + " " + typesInts[rightType]);
        }
        assign.setDirOut(st, leftOper);
        program.Add(assign);
    }
}

void check(SymbolTable st, int[] arr){
    string leftOper;
    string rightOper;
    int leftType;
    int rightType;
    int operat;
    string tempName;
    if(stackOperator.Count > 0){
        if(Array.Exists(arr, s => s.Equals(stackOperator.Peek()) )){
            // Get the data to create the quad
            rightType = stackTypes.Pop();
            rightOper = stackOperand.Pop();
            if(rightType == invalid){
                SemErr("Operand: " + rightOper + " not declared");
                return;
            }
            leftType = stackTypes.Pop();
            leftOper = stackOperand.Pop();
            if(leftType == invalid){
                SemErr("Operand: " + leftOper + " not declared");
                return;
            }
            operat = stackOperator.Pop();
            // Create the temporal variable
            tempName = "_t" + tempCont;
            tempCont+=1;
            Cuadruple quad = new Cuadruple(operat, leftOper, rightOper, tempName, st, operandInts);

            // Check if cube operator is valid for these operands
            if (quad.typeOut == invalid)
            {
                SemErr("Invalid operation: " + typesInts[leftType] + " " + operandInts[operat] + " " + typesInts[rightType]);
            }
            st.putSymbol(tempName, quad.typeOut, temporal, 1);
            quad.setDirOut(st, tempName);
            program.Add(quad);
            pushToOperandStack(tempName, st);
        }
    }
}

void checkInputOutput(SymbolTable st, int oper){
    string operand;
    int type;
    type = stackTypes.Pop();
    operand = stackOperand.Pop();
    if(type == invalid){
        SemErr("Variable: " + operand + " not declared");
        return;
    }
    InOut quad = new InOut(oper, operand, st, operandInts);
    quad.setDirOut(st, operand);
    program.Add(quad);
}

int checkArray(SymbolTable st, string name){
    int sizeDim = -1;
    stackOperand.Pop();
    stackTypes.Pop();
    int[] symbol = st.getSymbol(name);
    int varDims = symbol.Length-3;
    if(1 > varDims){
        SemErr("Variable " + name + " has " + varDims + " dimension, asked for 1");
    }else{
        sizeDim = symbol[3];
    }
    stackOperator.Push(_pl);
    return sizeDim;
}

int checkMatrix(SymbolTable st, string name){
    int sizeDim = -1;
    int[] symbol = st.getSymbol(name);
    int varDims = symbol.Length-3;
    if(2 > varDims){
        SemErr("Variable " + name + " has " + varDims + " dimension, asked for 2");
    }else{
        sizeDim = symbol[4];
    }
    return sizeDim;
}

void verifyLimit(SymbolTable st, string name, int sizeDim){
    string aux;
    string tempName,tempName1;
    string dim2;

    string pos = stackOperand.Peek(); // S1
    Verify ver = new Verify(pos, sizeDim-1, st);
    program.Add(ver);
    int[] symbol = st.getSymbol(name);
    if(symbol.Length > 4){
        // We have 2 dims
        aux = stackOperand.Pop();  //Result of expression
        dim2 = createConstInt(symbol[4],st);
        tempName1 = "_t" + tempCont;
        tempCont+=1;
        Cuadruple quad = new Cuadruple(_add, aux, createConstInt(1,st), tempName1, st, operandInts); //We need to add 1 to the S1 value to get the right dim
        st.putSymbol(tempName1, quad.typeOut, temporal, 1);
        quad.setDirOut(st, tempName1);
        program.Add(quad);
        tempName = "_t" + tempCont;
        tempCont+=1;
        Cuadruple quad1 = new Cuadruple(_mul,tempName1,dim2,tempName, st, operandInts);   // (S1+1) * Dim2
        st.putSymbol(tempName, quad.typeOut, temporal, 1);
        quad1.setDirOut(st, tempName);
        program.Add(quad1);
        pushToOperandStack(tempName, st);                                       //Top of operand is ^
    }
}

void verifyLimit2(SymbolTable st, string name, int sizeDim){
    string tempName, tempName1, dim2;

    if(sizeDim == -1){
        return;
    }

    string pos = stackOperand.Peek(); // S2
    Verify ver = new Verify(pos, sizeDim-1, st);
    program.Add(ver);
    
    string aux2 = stackOperand.Pop();       //S2
    string aux1 = stackOperand.Pop();       //(S1+1) * Dim2
    tempName = "_t" + tempCont;
    tempCont+=1;
    Cuadruple quad = new Cuadruple(_add,aux2,aux1,tempName, st, operandInts);   // (S1+1) * Dim2 + S2
    st.putSymbol(tempName, quad.typeOut, temporal, 1);
    quad.setDirOut(st, tempName);
    program.Add(quad);
    tempName1 = "_t" + tempCont;
    tempCont+=1;

    //Create D2
    int[] symbol = st.getSymbol(name);
    dim2 = createConstInt(symbol[4],st);

    Cuadruple quad1 = new Cuadruple(_sub,tempName,dim2,tempName1, st, operandInts);   // (S1+1) * Dim2 + S2 - Dim2
    st.putSymbol(tempName1, quad.typeOut, temporal, 1);
    quad1.setDirOut(st, tempName1);
    program.Add(quad1);

    pushToOperandStack(tempName1, st);                                       //Top of operand is (S1+1) * Dim2 + S2 - Dim2
}

void endArray(SymbolTable st, string name){
    string auxEnd;
    string tempDir,tempName;
    auxEnd =  stackOperand.Pop();

    tempDir = createConstInt(st.getDir(name),st);
    tempName = "_t" + tempCont;
    tempCont+=1;
    Cuadruple quad = new Cuadruple(_add,tempDir,auxEnd,tempName, st, operandInts);   // DimBase + res
    st.putSymbol(tempName, quad.typeOut, pointer, 1);
    quad.setDirOut(st, tempName);
    program.Add(quad);
    pushToOperandStack(tempName, st);
    stackOperator.Pop();
}

void makeIf(SymbolTable st){
    string cond;
    int typeCond;

    typeCond = stackTypes.Pop();
    cond = stackOperand.Pop();
    Goto GotoIf = new Goto(_gotoFalse, cond, st, operandInts);
    program.Add(GotoIf);
    stackJumps.Push(program.Count-1);
}

void makeIfEnd(){
    int endIf = stackJumps.Pop();
	Goto endJump = (Goto)program[endIf];
	endJump.setDirection(program.Count);
}

void makeElse(SymbolTable st){
    int falseIfIndex = stackJumps.Pop();
    Goto GotoEnd = new Goto(_goto, "", st, operandInts);
    program.Add(GotoEnd);
    stackJumps.Push(program.Count-1);
    Goto falseIf = (Goto)program[falseIfIndex];
    falseIf.setDirection(program.Count);
}

void makeLoop(SymbolTable st){
    string cond;
    int typeCond;

    typeCond = stackTypes.Pop();
    cond = stackOperand.Pop();
    Goto GotoWhile = new Goto(_gotoFalse, cond, st, operandInts);
    program.Add(GotoWhile);
    stackJumps.Push(program.Count-1);
}

void makeLoopEnd(SymbolTable st){
    int endWhile = stackJumps.Pop();
    int retEval = stackJumps.Pop();
	Goto evalJump = new Goto(_goto, "", st, operandInts);
    evalJump.setDirection(retEval);
    program.Add(evalJump);
    Goto gotoEndWhile = (Goto)program[endWhile];
	gotoEndWhile.setDirection(program.Count);
}

void makeFor(SymbolTable st){
    string cond;
    int typeCond;

    typeCond = stackTypes.Pop();
    cond = stackOperand.Pop();
    Goto GotoFalse = new Goto(_gotoFalse, cond, st, operandInts);
    program.Add(GotoFalse);
    stackJumps.Push(program.Count-1);
    Goto gotoTrue = new Goto(_goto, "", st, operandInts);
    program.Add(gotoTrue);
    stackJumps.Push(program.Count-1);
}

void forTrue(SymbolTable st){
    Goto gotoEval = new Goto(_goto, "", st, operandInts);
    program.Add(gotoEval);
    stackJumps.Push(program.Count-1);
}

void makeForEnd(SymbolTable st){
    int setGotoEval = stackJumps.Pop();
    int gotoBlockTrue = stackJumps.Pop();
    int gotoEnd = stackJumps.Pop();
    int retEval = stackJumps.Pop();
    Goto _gotoEnd = (Goto)program[gotoEnd];
    _gotoEnd.setDirection(program.Count+1);
	Goto gotoStep = new Goto(_goto, "", st, operandInts);
    gotoStep.setDirection(gotoBlockTrue+1);
    Goto _setGotoEval = (Goto)program[setGotoEval];
    _setGotoEval.setDirection(retEval);
    Goto _gotoBlockTrue = (Goto)program[gotoBlockTrue];
    _gotoBlockTrue.setDirection(setGotoEval+1);
    program.Add(gotoStep);
    
}

void checkClassCreation(string className){
    if(dirClasses.ContainsKey(className)){
        SemErr("Class <" + className + "> is already declared.");
        return;
    }
    dirClasses.Add(className, new Classes(program.Count + 1));
}

void addParentClass(string childClass, string parentClass){
    if(!dirClasses.ContainsKey(parentClass)){
        SemErr("Parent class <" + parentClass + "> is not declared.");
        return;
    }
    dirClasses[childClass].setParentClass(dirClasses[parentClass]);
}

void validateObject(string className){
    if(!dirClasses.ContainsKey(className)){
        SemErr("Object class <" + className + "> is not declared.");
        return;
    }
}

void createObject(string objName, string className, SymbolTable st){
    st.putObject(objName, dirClasses[className]);
}

void checkMethodCall(){

}

/*--------------------------------------------------------------------------*/    

bool IsFunctionCall(){
    scanner.ResetPeek();
    Token x = la; 
    while (x.kind == _id ) 
        x = scanner.Peek();
    return x.kind == _pl;
}

bool IsTypedFunctionCall(SymbolTable st){
    scanner.ResetPeek();
    Token x = la; 
    while (x.kind == _id ){
        if(!validateOper(la.val, st)){
                break;
        }
        else
        {
            if(st.getType(la.val) == t_void){
                SemErr("Invalid function call: " + _id + " does not return any value");
            }
            x = scanner.Peek();
        }
    }
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
    while (x.kind == _id || x.kind == _comma || x.kind == _br || x.kind == _bl || x.kind == _cte_I) 
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
		program.Add(new Goto(_goto, "", sTable, operandInts)); // Main GOTO. Always position 0
		
		while (StartOf(1)) {
			DECLARATION();
		}
		MAIN();
	}

	void DECLARATION() {
		if (IsTypeFunction() ) {
			DEC_FUNC("");
		} else if (StartOf(2)) {
			DEC_VARS(1);
		} else if (la.kind == 37) {
			DEC_CLASS();
		} else SynErr(53);
	}

	void MAIN() {
		sTable = sTable.newChildSymbolTable(); Goto mainGoto = (Goto)program[0]; mainGoto.setDirection(program.Count); 
		Expect(41);
		Expect(5);
		if (IsDecVars() ) {
			DEC_VARS(1);
		} else if (StartOf(3)) {
			STATUTE();
		} else SynErr(54);
		while (StartOf(4)) {
			if (IsDecVars() ) {
				DEC_VARS(1);
			} else {
				STATUTE();
			}
		}
		Expect(6);
		sTable = sTable.parentSymbolTable; 
	}

	void DEC_FUNC(string className) {
		string name; int type; bool solvedReturn; string returnVar; 
		TYPE_FUNC(out type );
		solvedReturn = (type == t_void); 
		IDENT(out name );
		sTable.putSymbol(name, type, func, 1);
		       dirFunc.Add(className+name, new Function(program.Count + 1));
		       sTable.putSymbol("_" + name, type, var, 1);
		       sTable = sTable.newChildSymbolTable(); 
		Expect(9);
		if (la.kind == 38 || la.kind == 39 || la.kind == 40) {
			PARAMS_FUNC(name, className);
		}
		Expect(10);
		Expect(5);
		if (StartOf(5)) {
			if (la.kind == 43) {
				RETURN(name, out returnVar);
				solvedReturn = true; checkReturn(sTable, "_" + name, returnVar); 
			} else if (IsDecVars() ) {
				DEC_VARS(1);
			} else {
				STATUTE();
			}
			while (StartOf(5)) {
				if (la.kind == 43) {
					RETURN(name, out returnVar);
					solvedReturn = true; checkReturn(sTable, "_" + name, returnVar); 
				} else if (IsDecVars() ) {
					DEC_VARS(1);
				} else {
					STATUTE();
				}
			}
		}
		if (!solvedReturn) { SemErr("Function requires return"); } 
		Expect(6);
		dirFunc[className+name].countVars(sTable);
		           sTable = sTable.parentSymbolTable;
		           program.Add(new EndFunc()); 
	}

	void DEC_VARS(int access) {
		string name; int type; string className; int dim1=0; int dim2=0;
		if (la.kind == 1) {
			IDENT(out className );
			validateObject(className); 
			IDENT(out name );
			sTable.putSymbol(name, t_obj, var, 1);  createObject(name, className, sTable); 
			while (la.kind == 11) {
				Get();
				IDENT(out name );
				sTable.putSymbol(name, t_obj, var, 1);  createObject(name, className, sTable); 
			}
			Expect(12);
		} else if (la.kind == 38 || la.kind == 39 || la.kind == 40) {
			SIMPLE_TYPE(out type );
			IDENT(out name );
			if (la.kind == 7) {
				Get();
				Expect(2);
				dim1 = Int32.Parse(t.val); 
				Expect(8);
				if (la.kind == 7) {
					Get();
					Expect(2);
					dim2 = Int32.Parse(t.val); 
					Expect(8);
				}
			}
			if(dim1>0){
			   sTable.putSymbolArray(name, type, var, dim1, dim2);
			   dim1 = 0;
			   dim2 = 0;
			}
			else
			   sTable.putSymbol(name, type, var, access);
			
			while (la.kind == 11) {
				Get();
				IDENT(out name );
				if (la.kind == 7) {
					Get();
					Expect(2);
					dim1 = Int32.Parse(t.val); 
					Expect(8);
					if (la.kind == 7) {
						Get();
						Expect(2);
						dim2 = Int32.Parse(t.val); 
						Expect(8);
					}
				}
				if(dim1>0){
				   sTable.putSymbolArray(name, type, var, dim1, dim2);
				   dim1 = 0;
				   dim2 = 0;
				}
				else
				   sTable.putSymbol(name, type, var, access);
				
			}
			Expect(12);
		} else SynErr(55);
	}

	void DEC_CLASS() {
		string className; string parentClassName;
		Expect(37);
		IDENT(out className );
		checkClassCreation(className); 
		if (la.kind == 28) {
			Get();
			IDENT(out parentClassName );
			addParentClass(className, parentClassName); 
		}
		sTable = sTable.newChildSymbolTable(); 
		Expect(5);
		while (la.kind == 13 || la.kind == 14) {
			CLASS_DEF(className );
		}
		Expect(6);
		dirClasses[className].setClassVars(sTable);
		SymbolTable aux = sTable;
		sTable = sTable.parentSymbolTable;
		sTable.objectsContext[className] = aux;
		
	}

	void IDENT(out string name ) {
		Expect(1);
		name = t.val; 
	}

	void SIMPLE_TYPE(out int type ) {
		type = undef; 
		if (la.kind == 38) {
			Get();
			type = t_int; 
		} else if (la.kind == 39) {
			Get();
			type = t_float; 
		} else if (la.kind == 40) {
			Get();
			type = t_char; 
		} else SynErr(56);
	}

	void TYPE_FUNC(out int type ) {
		type = undef; 
		if (la.kind == 38) {
			Get();
			type = t_int; 
		} else if (la.kind == 39) {
			Get();
			type = t_float; 
		} else if (la.kind == 40) {
			Get();
			type = t_char; 
		} else if (la.kind == 42) {
			Get();
			type = t_void; 
		} else SynErr(57);
	}

	void PARAMS_FUNC(string currFunc, string className ) {
		string name; int type; 
		SIMPLE_TYPE(out type );
		IDENT(out name );
		sTable.putSymbol(name, type, var, 1);
		       dirFunc[className+currFunc].parameterTypes.Add(type); 
		while (la.kind == 11) {
			Get();
			SIMPLE_TYPE(out type );
			IDENT(out name );
			sTable.putSymbol(name, type, var, 1);
			      dirFunc[currFunc].parameterTypes.Add(type); 
		}
	}

	void RETURN(string funcName, out string returnVar ) {
		Expect(43);
		HYPER_EXP();
		Expect(12);
		returnVar = stackOperand.Peek(); program.Add(new Return(stackOperand.Pop(), sTable.getDir("_"+funcName), sTable)); 
	}

	void STATUTE() {
		if (la.kind == 44) {
			INPUT();
		} else if (la.kind == 45) {
			PRINT();
		} else if (IsFunctionCall() ) {
			FUNC_CALL();
			Expect(12);
		} else if (IsMethodCall() ) {
			METHOD_CALL();
		} else if (la.kind == 46) {
			CONDITIONAL();
		} else if (la.kind == 48) {
			WHILE();
		} else if (la.kind == 49) {
			FOR();
		} else if (la.kind == 1) {
			ASSIGN();
		} else SynErr(58);
	}

	void CLASS_DEF(string className) {
		int access = 1;
		if (la.kind == 13) {
			Get();
			access = 1; 
		} else if (la.kind == 14) {
			Get();
			access = -1; 
		} else SynErr(59);
		if (IsTypeFunction() ) {
			DEC_FUNC(className+".");
		} else if (StartOf(2)) {
			DEC_VARS(access);
		} else SynErr(60);
	}

	void INPUT() {
		Expect(44);
		Expect(9);
		VARIABLE_ASSIGN();
		checkInputOutput(sTable, _input); 
		Expect(10);
		Expect(12);
	}

	void PRINT() {
		Expect(45);
		Expect(9);
		HYPER_EXP();
		checkInputOutput(sTable, _print); 
		while (la.kind == 11) {
			Get();
			HYPER_EXP();
			checkInputOutput(sTable, _print); 
		}
		Expect(10);
		Expect(12);
	}

	void FUNC_CALL() {
		string name; int paramCount = 0; string localParamType; string funcParamType;
		IDENT(out name );
		if (sTable.getSymbol(name) == null) { SemErr("Function does not exists"); }
		 program.Add(new Era(name)); 
		Expect(9);
		if (StartOf(6)) {
			HYPER_EXP();
			funcParamType = typesInts[dirFunc[name].parameterTypes[paramCount]];
			localParamType = typesInts[sTable.getType(stackOperand.Peek())];
			if (localParamType  != funcParamType) { 
			   SemErr("Parameter type mismatch. Expected <" + funcParamType + ">. Found <" + localParamType + ">"); 
			} 
			program.Add(new Param(stackOperand.Pop(), paramCount, sTable)); 
			paramCount ++; 
			
			while (la.kind == 11) {
				Get();
				HYPER_EXP();
				if(paramCount >= dirFunc[name].parameterTypes.Count){
				   SemErr("Parameter number mismatch. Expected Just " + dirFunc[name].parameterTypes.Count + " Parameters. Found more");
				}else{
				   funcParamType = typesInts[dirFunc[name].parameterTypes[paramCount]];
				   localParamType = typesInts[sTable.getType(stackOperand.Peek())];
				   if (localParamType  != funcParamType) { 
				       SemErr("Parameter type mismatch. Expected <" + funcParamType + ">. Found <" + localParamType + ">"); 
				   } 
				   program.Add(new Param(stackOperand.Pop(), paramCount, sTable)); 
				   paramCount ++;
				}
				
			}
		}
		Expect(10);
		if(paramCount < dirFunc[name].parameterTypes.Count){
		   SemErr("Parameter number mismatch. Expected " + dirFunc[name].parameterTypes.Count + " Parameters. Found " + paramCount + ""); 
		}
		program.Add(new GoSub(name)); 
		// If not void create temp to store result of call
		if(sTable.getType(name) != t_void){
		   pushToOperandStack(createTemp(sTable.getType(name), sTable), sTable);
		   string leftOper = stackOperand.Peek();
		   Assign assign = new Assign(_equal, "_"+name, leftOper, sTable, operandInts);
		   assign.setDirOut(sTable, leftOper);
		   program.Add(assign);
		}
		
	}

	void METHOD_CALL() {
		string objectName, methodName; 
		IDENT(out objectName );
		Expect(21);
		IDENT(out methodName );
		Expect(9);
		if (StartOf(6)) {
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
		Expect(46);
		Expect(9);
		HYPER_EXP();
		makeIf(sTable); 
		Expect(10);
		BLOCK();
		if (la.kind == 47) {
			Get();
			makeElse(sTable); 
			BLOCK();
		}
		makeIfEnd(); 
	}

	void WHILE() {
		Expect(48);
		Expect(9);
		stackJumps.Push(program.Count); 
		HYPER_EXP();
		makeLoop(sTable); 
		Expect(10);
		BLOCK();
		makeLoopEnd(sTable); 
	}

	void FOR() {
		Expect(49);
		Expect(9);
		ASSIGN();
		stackJumps.Push(program.Count); 
		HYPER_EXP();
		makeFor(sTable); 
		Expect(12);
		ASSIGN();
		forTrue(sTable); 
		Expect(10);
		BLOCK();
		makeForEnd(sTable); 
	}

	void ASSIGN() {
		VARIABLE_ASSIGN();
		if (StartOf(7)) {
			if (StartOf(8)) {
				SHORT_ASSIGN();
			} else {
				Get();
				stackOperator.Push(_equal); 
			}
			HYPER_EXP();
		} else if (la.kind == 26 || la.kind == 27) {
			STEP();
		} else SynErr(61);
		Expect(12);
		checkAssign(sTable); 
	}

	void HYPER_EXP() {
		SUPER_EXP();
		while (StartOf(9)) {
			REL_EXP();
			SUPER_EXP();
			check(sTable, RELEXP_OPERATORS); 
		}
		check(sTable, RELEXP_OPERATORS); 
	}

	void EXP() {
		TERM();
		while (la.kind == 13 || la.kind == 14) {
			if (la.kind == 13) {
				Get();
				stackOperator.Push(_add); 
			} else {
				Get();
				stackOperator.Push(_sub);
			}
			TERM();
			check(sTable, EXP_OPERATORS); 
		}
		check(sTable, EXP_OPERATORS); 
	}

	void TERM() {
		FACT();
		while (StartOf(10)) {
			OPERATORS_TERM();
			FACT();
			check(sTable, TERM_OPERATORS); 
		}
		check(sTable, TERM_OPERATORS); 
	}

	void VARIABLE_ASSIGN() {
		string name; string attrName; int dim1Size=0; int dim2Size=0;
		IDENT(out name );
		pushToOperandStack(name, sTable); 
		if (la.kind == 7 || la.kind == 21) {
			if (la.kind == 21) {
				Get();
				IDENT(out attrName );
				stackOperand.Pop(); stackTypes.Pop(); pushToOperandStack(name+"."+attrName, sTable); 
			} else {
				Get();
				dim1Size = checkArray(sTable, name); 
				EXP();
				verifyLimit(sTable, name, dim1Size); 
				Expect(8);
				if (la.kind == 7) {
					dim2Size = checkMatrix(sTable, name); 
					Get();
					EXP();
					verifyLimit2(sTable, name, dim2Size); 
					Expect(8);
				}
				endArray(sTable, name); 
			}
		}
	}

	void SHORT_ASSIGN() {
		if (la.kind == 22) {
			Get();
			stackOperator.Push(_sadd); 
		} else if (la.kind == 23) {
			Get();
			stackOperator.Push(_ssub); 
		} else if (la.kind == 25) {
			Get();
			stackOperator.Push(_smul); 
		} else if (la.kind == 24) {
			Get();
			stackOperator.Push(_sdiv); 
		} else SynErr(62);
	}

	void STEP() {
		if (la.kind == 26) {
			Get();
			stackOperator.Push(_increment); 
		} else if (la.kind == 27) {
			Get();
			stackOperator.Push(_decrement); 
		} else SynErr(63);
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
			stackOperator.Push(_pl); 
			HYPER_EXP();
			Expect(10);
			stackOperator.Pop(); 
		} else if (StartOf(11)) {
			if (la.kind == 13 || la.kind == 14) {
				if (la.kind == 13) {
					Get();
				} else {
					Get();
				}
			}
			if (la.kind == 2) {
				Get();
				pushToOperandStack(createConstInt(Int32.Parse(t.val), sTable), sTable); 
			} else if (la.kind == 3) {
				Get();
				pushToOperandStack(createConstFloat(float.Parse(t.val), sTable), sTable); 
			} else SynErr(64);
		} else if (la.kind == 4) {
			Get();
			pushToOperandStack(createConstString(t.val, sTable), sTable); 
		} else if (la.kind == 1) {
			VARIABLE_FACT();
		} else SynErr(65);
	}

	void OPERATORS_TERM() {
		if (la.kind == 15) {
			Get();
			stackOperator.Push(_mul); 
		} else if (la.kind == 17) {
			Get();
			stackOperator.Push(_div); 
		} else if (la.kind == 16) {
			Get();
			stackOperator.Push(_exponent); 
		} else if (la.kind == 18) {
			Get();
			stackOperator.Push(_intdiv); 
		} else if (la.kind == 19) {
			Get();
			stackOperator.Push(_module); 
		} else SynErr(66);
	}

	void VARIABLE_FACT() {
		string name; string attrName; int dim1Size=0; int dim2Size=0;
		if (IsTypedFunctionCall(sTable) ) {
			stackOperator.Push(_pl); 
			FUNC_CALL();
			stackOperator.Pop(); 
		} else if (la.kind == 1) {
			IDENT(out name );
			pushToOperandStack(name, sTable); 
			if (la.kind == 7 || la.kind == 21) {
				if (la.kind == 21) {
					Get();
					IDENT(out attrName );
					stackOperand.Pop(); stackTypes.Pop(); pushToOperandStack(name+"."+attrName, sTable); 
				} else {
					Get();
					dim1Size = checkArray(sTable, name); 
					EXP();
					verifyLimit(sTable, name, dim1Size); 
					Expect(8);
					if (la.kind == 7) {
						dim2Size = checkMatrix(sTable, name); 
						Get();
						EXP();
						verifyLimit2(sTable, name, dim2Size); 
						Expect(8);
					}
					endArray(sTable, name); 
				}
			}
		} else SynErr(67);
	}

	void SUPER_EXP() {
		EXP();
		while (StartOf(12)) {
			REL_OP();
			EXP();
			check(sTable, RELOP_OPERATORS); 
		}
		check(sTable, RELOP_OPERATORS); 
	}

	void REL_EXP() {
		if (la.kind == 50) {
			Get();
			stackOperator.Push(_and); 
		} else if (la.kind == 35) {
			Get();
			stackOperator.Push(_and); 
		} else if (la.kind == 51) {
			Get();
			stackOperator.Push(_or); 
		} else if (la.kind == 36) {
			Get();
			stackOperator.Push(_or); 
		} else SynErr(68);
	}

	void REL_OP() {
		if (la.kind == 29 || la.kind == 30) {
			if (la.kind == 30) {
				Get();
				stackOperator.Push(_greater); 
			} else {
				Get();
				stackOperator.Push(_less); 
			}
		} else if (la.kind == 31 || la.kind == 32) {
			if (la.kind == 32) {
				Get();
				stackOperator.Push(_greatereq); 
			} else {
				Get();
				stackOperator.Push(_lesseq); 
			}
		} else if (la.kind == 33 || la.kind == 34) {
			if (la.kind == 33) {
				Get();
				stackOperator.Push(_equaleq); 
			} else {
				Get();
				stackOperator.Push(_different); 
			}
		} else SynErr(69);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		PROGRAM();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_T, _T,_T,_T,_x, _T,_T,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x}

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
			case 16: s = "exponent expected"; break;
			case 17: s = "div expected"; break;
			case 18: s = "intdiv expected"; break;
			case 19: s = "module expected"; break;
			case 20: s = "equal expected"; break;
			case 21: s = "dot expected"; break;
			case 22: s = "sadd expected"; break;
			case 23: s = "ssub expected"; break;
			case 24: s = "sdiv expected"; break;
			case 25: s = "smul expected"; break;
			case 26: s = "increment expected"; break;
			case 27: s = "decrement expected"; break;
			case 28: s = "colon expected"; break;
			case 29: s = "less expected"; break;
			case 30: s = "greater expected"; break;
			case 31: s = "lesseq expected"; break;
			case 32: s = "greatereq expected"; break;
			case 33: s = "equaleq expected"; break;
			case 34: s = "different expected"; break;
			case 35: s = "and expected"; break;
			case 36: s = "or expected"; break;
			case 37: s = "\"class\" expected"; break;
			case 38: s = "\"int\" expected"; break;
			case 39: s = "\"float\" expected"; break;
			case 40: s = "\"char\" expected"; break;
			case 41: s = "\"main\" expected"; break;
			case 42: s = "\"void\" expected"; break;
			case 43: s = "\"return\" expected"; break;
			case 44: s = "\"input\" expected"; break;
			case 45: s = "\"print\" expected"; break;
			case 46: s = "\"if\" expected"; break;
			case 47: s = "\"else\" expected"; break;
			case 48: s = "\"while\" expected"; break;
			case 49: s = "\"for\" expected"; break;
			case 50: s = "\"and\" expected"; break;
			case 51: s = "\"or\" expected"; break;
			case 52: s = "??? expected"; break;
			case 53: s = "invalid DECLARATION"; break;
			case 54: s = "invalid MAIN"; break;
			case 55: s = "invalid DEC_VARS"; break;
			case 56: s = "invalid SIMPLE_TYPE"; break;
			case 57: s = "invalid TYPE_FUNC"; break;
			case 58: s = "invalid STATUTE"; break;
			case 59: s = "invalid CLASS_DEF"; break;
			case 60: s = "invalid CLASS_DEF"; break;
			case 61: s = "invalid ASSIGN"; break;
			case 62: s = "invalid SHORT_ASSIGN"; break;
			case 63: s = "invalid STEP"; break;
			case 64: s = "invalid FACT"; break;
			case 65: s = "invalid FACT"; break;
			case 66: s = "invalid OPERATORS_TERM"; break;
			case 67: s = "invalid VARIABLE_FACT"; break;
			case 68: s = "invalid REL_EXP"; break;
			case 69: s = "invalid REL_OP"; break;

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
