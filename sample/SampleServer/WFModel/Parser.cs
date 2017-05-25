
//#define POSITIONS

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using CocoRCore;

namespace CocoRCore.Samples.WFModel {



	public class Parser : ParserBase 
	{
	public const int _EOF = 0; // TOKEN EOF
	public const int _ident = 1; // TOKEN ident
	public const int _dottedident = 2; // TOKEN dottedident
	public const int _number = 3; // TOKEN number
	public const int _int = 4; // TOKEN int
	public const int _string = 5; // TOKEN string
	public const int _braced = 6; // TOKEN braced
	public const int _bracketed = 7; // TOKEN bracketed
	public const int _end = 8; // TOKEN end
	public const int _dot = 9; // TOKEN dot
	public const int _bar = 10; // TOKEN bar
	public const int _colon = 11; // TOKEN colon
	public const int _versionnumber = 12; // TOKEN versionnumber
	public const int _version = 13; // TOKEN version INHERITS ident
	public const int _search = 14; // TOKEN search INHERITS ident
	public const int _select = 15; // TOKEN select INHERITS ident
	public const int _details = 16; // TOKEN details INHERITS ident
	public const int _edit = 17; // TOKEN edit INHERITS ident
	public const int _clear = 18; // TOKEN clear INHERITS ident
	public const int _keys = 19; // TOKEN keys INHERITS ident
	public const int _displayname = 20; // TOKEN displayname INHERITS ident
	public const int _vbident = 21; // TOKEN vbident INHERITS ident
	private const int __maxT = 72;

		private const bool _T = true;
		private const bool _x = false;
	public readonly Symboltable types;
	public readonly Symboltable enumtypes;
	public Symboltable symbols(string name) {
		if (name == "types") return types;
		if (name == "enumtypes") return enumtypes;
		return null;
	}

public override void Prime(ref Token t) { 
if (t.kind == _string || t.kind == _braced || t.kind == _bracketed)
{
	var tb = t.Copy(); 
	tb.setValue(t.valScanned.Substring(1, t.val.Length - 2), scanner.casingString);
	t = tb.Freeze();
}
}



		public Parser(Scanner scanner) : base(scanner, new Errors())
		{
		types = new Symboltable("types", true, false, this);
		enumtypes = new Symboltable("enumtypes", true, false, this);
		astbuilder = new AST.Builder(this);
		}

		public override int maxT => __maxT;

		protected override void Get() 
		{
			for (;;) 
			{
				t = la;

				if (alternatives != null) 
				{
					tokens.Add(new Alternative(t, alternatives));
				}
				_newAlt();

				la = scanner.Scan();
				if (la.kind <= maxT) { ++errDist; break; }

				la = t;
			}
		}

        private bool isKind(Token t, int n)
        {
            var k = t.kind;
            while (k >= 0)
            {
                if (k == n) return true;
                k = tBase[k];
            }
            return false;
        }

        // is the lookahead token la a start of the production s?
        private bool StartOf(int s)
        {
            return set[s, la.kind];
        }

        private bool WeakSeparator(int n, int syFol, int repFol)
        {
            var kind = la.kind;
            if (isKind(la, n)) { Get(); return true; }
            else if (StartOf(repFol)) { return false; }
            else
            {
                SynErr(n);
                while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
                {
                    Get();
                    kind = la.kind;
                }
                return StartOf(syFol);
            }
        }

        protected void Expect(int n)
        {
            if (isKind(la, n)) Get(); else { SynErr(n); }
        }


        protected void ExpectWeak(int n, int follow)
        {
            if (isKind(la, n)) Get();
            else
            {
                SynErr(n);
                while (!StartOf(follow)) Get();
            }
        }



	void WFModel‿NT() {
		using(astbuilder.createBarrier(null))
		{
		using(astbuilder.createMarker("version", null, false, false, false))
		Version‿NT();
		using(astbuilder.createMarker("namespace", null, false, false, false))
		Namespace‿NT();
		addAlt(23); // OPT
		if (isKind(la, 23)) {
			using(astbuilder.createMarker("readerwriterprefix", null, false, false, false))
			ReaderWriterPrefix‿NT();
		}
		using(astbuilder.createMarker("rootclass", null, false, false, false))
		RootClass‿NT();
		addAlt(set0, 1); // ITER start
		while (StartOf(1)) {
			addAlt(26); // ALT
			addAlt(62); // ALT
			addAlt(70); // ALT
			addAlt(69); // ALT
			if (isKind(la, 26)) {
				using(astbuilder.createMarker("class", null, true, false, false))
				Class‿NT();
			} else if (isKind(la, 62)) {
				using(astbuilder.createMarker("subsystem", null, true, false, false))
				SubSystem‿NT();
			} else if (isKind(la, 70)) {
				using(astbuilder.createMarker("enum", null, true, false, false))
				Enum‿NT();
			} else {
				using(astbuilder.createMarker("flags", null, true, false, false))
				Flags‿NT();
			}
			addAlt(set0, 1); // ITER end
		}
		EndNamespace‿NT();
	}}

	void Version‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(13); // T
		Expect(13); // version
		addAlt(12); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(12); // versionnumber
	}}

	void Namespace‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 22))) {SynErr(73); Get();}
		addAlt(22); // T
		Expect(22); // "namespace"
		DottedIdent‿NT();
	}}

	void ReaderWriterPrefix‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 23))) {SynErr(74); Get();}
		addAlt(23); // T
		Expect(23); // "readerwriterprefix"
		addAlt(1); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(1); // ident
	}}

	void RootClass‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 24))) {SynErr(75); Get();}
		addAlt(24); // T
		using(astbuilder.createMarker("typ", null, false, true, false))
		Expect(24); // "rootclass"
		addAlt(25); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(25); // "data"
		using(astbuilder.createMarker("properties", null, true, false, false))
		Properties‿NT();
		addAlt(8); // T
		Expect(8); // end
		addAlt(26); // T
		Expect(26); // "class"
	}}

	void Class‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 26))) {SynErr(76); Get();}
		addAlt(26); // T
		using(astbuilder.createMarker("typ", null, false, true, false))
		Expect(26); // "class"
		if (!types.Add(la)) SemErr(71, la, string.Format(DuplicateSymbol, "ident", la.val, types.name));
		alternatives.tdeclares = types;
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		addAlt(6); // OPT
		if (isKind(la, 6)) {
			using(astbuilder.createMarker("title", null, false, false, false))
			Title‿NT();
		}
		addAlt(28); // OPT
		if (isKind(la, 28)) {
			using(astbuilder.createMarker("inherits", null, false, false, false))
			Inherits‿NT();
		}
		addAlt(27); // OPT
		if (isKind(la, 27)) {
			using(astbuilder.createMarker("via", null, false, false, false))
			Via‿NT();
		}
		using(astbuilder.createMarker("properties", null, true, false, false))
		Properties‿NT();
		addAlt(8); // T
		Expect(8); // end
		addAlt(26); // T
		Expect(26); // "class"
	}}

	void SubSystem‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 62))) {SynErr(77); Get();}
		addAlt(62); // T
		Expect(62); // "subsystem"
		if (!types.Add(la)) SemErr(71, la, string.Format(DuplicateSymbol, "ident", la.val, types.name));
		alternatives.tdeclares = types;
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		addAlt(63); // T
		Expect(63); // "ssname"
		addAlt(1); // T
		using(astbuilder.createMarker("ssname", null, false, true, false))
		Expect(1); // ident
		addAlt(64); // T
		Expect(64); // "ssconfig"
		addAlt(1); // T
		using(astbuilder.createMarker("ssconfig", null, false, true, false))
		Expect(1); // ident
		addAlt(65); // T
		Expect(65); // "sstyp"
		addAlt(1); // T
		using(astbuilder.createMarker("sstyp", null, false, true, false))
		Expect(1); // ident
		addAlt(66); // T
		Expect(66); // "sscommands"
		using(astbuilder.createMarker("sscommands", null, true, false, false))
		SSCommands‿NT();
		addAlt(67); // OPT
		if (isKind(la, 67)) {
			Get();
			addAlt(5); // T
			using(astbuilder.createMarker("sskey", null, false, true, true))
			Expect(5); // string
		}
		addAlt(68); // OPT
		if (isKind(la, 68)) {
			Get();
			addAlt(5); // T
			using(astbuilder.createMarker("ssclear", null, false, true, true))
			Expect(5); // string
		}
		addAlt(30); // ITER start
		while (isKind(la, 30)) {
			using(astbuilder.createMarker("properties", null, true, false, false))
			InfoProperty‿NT();
			addAlt(30); // ITER end
		}
		addAlt(8); // T
		Expect(8); // end
		addAlt(62); // T
		Expect(62); // "subsystem"
	}}

	void Enum‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 70))) {SynErr(78); Get();}
		addAlt(70); // T
		Expect(70); // "enum"
		if (!enumtypes.Add(la)) SemErr(71, la, string.Format(DuplicateSymbol, "ident", la.val, enumtypes.name));
		alternatives.tdeclares = enumtypes;
		addAlt(1); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(1); // ident
		EnumValues‿NT();
		addAlt(8); // T
		Expect(8); // end
		addAlt(70); // T
		Expect(70); // "enum"
	}}

	void Flags‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(isKind(la, 0) || isKind(la, 69))) {SynErr(79); Get();}
		addAlt(69); // T
		Expect(69); // "flags"
		if (!types.Add(la)) SemErr(71, la, string.Format(DuplicateSymbol, "ident", la.val, types.name));
		alternatives.tdeclares = types;
		addAlt(1); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(1); // ident
		addAlt(1); // ITER start
		while (isKind(la, 1)) {
			EnumValue‿NT();
			addAlt(1); // ITER end
		}
		addAlt(8); // T
		Expect(8); // end
		addAlt(69); // T
		Expect(69); // "flags"
	}}

	void EndNamespace‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(8); // T
		Expect(8); // end
		addAlt(22); // T
		Expect(22); // "namespace"
	}}

	void DottedIdent‿NT() {
		using(astbuilder.createBarrier("."))
		{
		addAlt(2); // OPT
		if (isKind(la, 2)) {
			using(astbuilder.createMarker(null, null, false, true, false))
			Get();
			addAlt(9); // T
			Expect(9); // dot
			addAlt(2); // ITER start
			while (isKind(la, 2)) {
				using(astbuilder.createMarker(null, null, false, true, false))
				Get();
				addAlt(9); // T
				Expect(9); // dot
				addAlt(2); // ITER end
			}
		}
		addAlt(1); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(1); // ident
	}}

	void DottedIdentBare‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(2); // OPT
		if (isKind(la, 2)) {
			Get();
			addAlt(9); // T
			Expect(9); // dot
			addAlt(2); // ITER start
			while (isKind(la, 2)) {
				Get();
				addAlt(9); // T
				Expect(9); // dot
				addAlt(2); // ITER end
			}
		}
		addAlt(1); // T
		Expect(1); // ident
	}}

	void Properties‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(set0, 2); // ITER start
		while (StartOf(2)) {
			Prop‿NT();
			addAlt(set0, 2); // ITER end
		}
	}}

	void Title‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(6); // T
		using(astbuilder.createMarker(null, null, false, true, false))
		Expect(6); // braced
	}}

	void Inherits‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(28); // T
		Expect(28); // "inherits"
		DottedIdent‿NT();
	}}

	void Via‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(27); // T
		Expect(27); // "via"
		DottedIdent‿NT();
	}}

	void Prop‿NT() {
		using(astbuilder.createBarrier(null))
		{
		while (!(StartOf(3))) {SynErr(80); Get();}
		addAlt(29); // ALT
		addAlt(30); // ALT
		addAlt(31); // ALT
		addAlt(32); // ALT
		addAlt(33); // ALT
		addAlt(34); // ALT
		addAlt(35); // ALT
		addAlt(36); // ALT
		switch (la.kind) {
		case 29: // "property"
		{
			Property‿NT();
			break;
		}
		case 30: // "infoproperty"
		{
			InfoProperty‿NT();
			break;
		}
		case 31: // "approperty"
		{
			APProperty‿NT();
			break;
		}
		case 32: // "list"
		{
			List‿NT();
			break;
		}
		case 33: // "selectlist"
		{
			SelectList‿NT();
			break;
		}
		case 34: // "flagslist"
		{
			FlagsList‿NT();
			break;
		}
		case 35: // "longproperty"
		{
			LongProperty‿NT();
			break;
		}
		case 36: // "infolongproperty"
		{
			InfoLongProperty‿NT();
			break;
		}
		default: SynErr(81); break;
		}
	}}

	void Property‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(29); // T
		using(astbuilder.createMarker("writeable", "t", false, true, false))
		Expect(29); // "property"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		using(astbuilder.createMarker("type", null, false, false, false))
		Type‿NT();
	}}

	void InfoProperty‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(30); // T
		using(astbuilder.createMarker("writeable", "f", false, true, false))
		Expect(30); // "infoproperty"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		using(astbuilder.createMarker("type", null, false, false, false))
		Type‿NT();
	}}

	void APProperty‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(31); // T
		using(astbuilder.createMarker("writeable", "t", false, true, false))
		using(astbuilder.createMarker("autopostback", "t", false, true, false))
		Expect(31); // "approperty"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		using(astbuilder.createMarker("type", null, false, false, false))
		Type‿NT();
	}}

	void List‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(32); // T
		using(astbuilder.createMarker("list", "t", false, true, false))
		Expect(32); // "list"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		addAlt(41); // OPT
		if (isKind(la, 41)) {
			using(astbuilder.createMarker("type", null, false, false, false))
			As‿NT();
		}
	}}

	void SelectList‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(33); // T
		using(astbuilder.createMarker("list", "t", false, true, false))
		using(astbuilder.createMarker("select", "t", false, true, false))
		Expect(33); // "selectlist"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		using(astbuilder.createMarker("type", null, false, false, false))
		As‿NT();
	}}

	void FlagsList‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(34); // T
		using(astbuilder.createMarker("list", "t", false, true, false))
		using(astbuilder.createMarker("flags", "t", false, true, false))
		Expect(34); // "flagslist"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
		using(astbuilder.createMarker("type", null, false, false, false))
		Mimics‿NT();
	}}

	void LongProperty‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(35); // T
		using(astbuilder.createMarker("writeable", "t", false, true, false))
		using(astbuilder.createMarker("long", "t", false, true, false))
		Expect(35); // "longproperty"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
	}}

	void InfoLongProperty‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(36); // T
		using(astbuilder.createMarker("writeable", "f", false, true, false))
		using(astbuilder.createMarker("long", "t", false, true, false))
		Expect(36); // "infolongproperty"
		addAlt(1); // T
		using(astbuilder.createMarker("name", null, false, true, false))
		Expect(1); // ident
	}}

	void Type‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(set0, 4); // ALT
		addAlt(41); // ALT
		addAlt(57); // ALT
		if (StartOf(4)) {
			using(astbuilder.createMarker("basic", "String", false, true, false))
			EmptyType‿NT();
		} else if (isKind(la, 41)) {
			As‿NT();
		} else if (isKind(la, 57)) {
			Mimics‿NT();
		} else SynErr(82);
		addAlt(37); // OPT
		if (isKind(la, 37)) {
			Get();
			using(astbuilder.createMarker("initvalue", null, false, false, false))
			InitValue‿NT();
		}
		addAlt(6); // OPT
		if (isKind(la, 6)) {
			using(astbuilder.createMarker("samplevalue", null, false, false, false))
			SampleValue‿NT();
		}
	}}

	void As‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(41); // T
		Expect(41); // "as"
		addAlt(set0, 5); // ALT
		addAlt(1); // ALT
		addAlt(1, types); // ALT ident uses symbol table 'types'
		addAlt(new int[] {1, 2}); // ALT
		if (StartOf(5)) {
			BaseType‿NT();
		} else if (isKind(la, 1)) {
			if (!types.Use(la, alternatives)) SemErr(72, la, string.Format(MissingSymbol, "ident", la.val, types.name));
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
		} else if (isKind(la, 1) || isKind(la, 2)) {
			using(astbuilder.createMarker("basic", null, false, false, false))
			DottedIdent‿NT();
		} else SynErr(83);
	}}

	void Mimics‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(57); // T
		using(astbuilder.createMarker("basic", "String", false, true, false))
		Expect(57); // "mimics"
		addAlt(set0, 6); // ALT
		addAlt(1); // ALT
		addAlt(1, enumtypes); // ALT ident uses symbol table 'enumtypes'
		if (StartOf(6)) {
			using(astbuilder.createMarker("mimicsspec", null, false, false, false))
			MimicsSpec‿NT();
		} else if (isKind(la, 1)) {
			if (!enumtypes.Use(la, alternatives)) SemErr(72, la, string.Format(MissingSymbol, "ident", la.val, enumtypes.name));
			using(astbuilder.createMarker("mimicsspec", null, false, true, false))
			using(astbuilder.createMarker("enum", "t", false, true, false))
			Get();
		} else SynErr(84);
	}}

	void EmptyType‿NT() {
		using(astbuilder.createBarrier(null))
		{
	}}

	void InitValue‿NT() {
		using(astbuilder.createBarrier(""))
		{
		addAlt(3); // ALT
		addAlt(4); // ALT
		addAlt(5); // ALT
		addAlt(38); // ALT
		addAlt(39); // ALT
		addAlt(40); // ALT
		addAlt(new int[] {1, 2}); // ALT
		switch (la.kind) {
		case 3: // number
		{
			Get();
			break;
		}
		case 4: // int
		{
			Get();
			break;
		}
		case 5: // string
		{
			Get();
			break;
		}
		case 38: // "true"
		{
			Get();
			break;
		}
		case 39: // "false"
		{
			Get();
			break;
		}
		case 40: // "#"
		{
			Get();
			addAlt(set0, 7); // ITER start
			while (StartOf(7)) {
				Get();
				addAlt(set0, 7); // ITER end
			}
			addAlt(40); // T
			Expect(40); // "#"
			break;
		}
		case 1: // ident
		case 2: // dottedident
		case 13: // version
		case 14: // search
		case 15: // select
		case 16: // details
		case 17: // edit
		case 18: // clear
		case 19: // keys
		case 20: // displayname
		case 21: // vbident
		{
			FunctionCall‿NT();
			break;
		}
		default: SynErr(85); break;
		}
	}}

	void SampleValue‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(6); // T
		using(astbuilder.createMarker(null, null, false, true, true))
		Expect(6); // braced
	}}

	void FunctionCall‿NT() {
		using(astbuilder.createBarrier(null))
		{
		DottedIdentBare‿NT();
		addAlt(7); // T
		Expect(7); // bracketed
	}}

	void BaseType‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(42); // ALT
		addAlt(43); // ALT
		addAlt(44); // ALT
		addAlt(45); // ALT
		addAlt(46); // ALT
		addAlt(47); // ALT
		addAlt(48); // ALT
		addAlt(49); // ALT
		addAlt(50); // ALT
		addAlt(51); // ALT
		addAlt(52); // ALT
		addAlt(53); // ALT
		addAlt(54); // ALT
		addAlt(55); // ALT
		addAlt(56); // ALT
		switch (la.kind) {
		case 42: // "double"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "c", false, true, false))
			Get();
			break;
		}
		case 43: // "date"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "d", false, true, false))
			Get();
			break;
		}
		case 44: // "datetime"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "{0:d} {0:t}", false, true, false))
			Get();
			break;
		}
		case 45: // "integer"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "n0", false, true, false))
			Get();
			break;
		}
		case 46: // "percent"
		{
			using(astbuilder.createMarker("basic", "double", false, true, false))
			using(astbuilder.createMarker("format", "p", false, true, false))
			Get();
			break;
		}
		case 47: // "percentwithdefault"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "p", false, true, false))
			Get();
			break;
		}
		case 48: // "doublewithdefault"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "c", false, true, false))
			Get();
			break;
		}
		case 49: // "integerwithdefault"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			using(astbuilder.createMarker("format", "n0", false, true, false))
			Get();
			break;
		}
		case 50: // "n2"
		{
			using(astbuilder.createMarker("basic", "double", false, true, false))
			using(astbuilder.createMarker("format", "n2", false, true, false))
			Get();
			break;
		}
		case 51: // "n0"
		{
			using(astbuilder.createMarker("basic", "integer", false, true, false))
			using(astbuilder.createMarker("format", "n0", false, true, false))
			Get();
			break;
		}
		case 52: // "string"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
			break;
		}
		case 53: // "boolean"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
			break;
		}
		case 54: // "guid"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
			break;
		}
		case 55: // "string()"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
			break;
		}
		case 56: // "xml"
		{
			using(astbuilder.createMarker("basic", null, false, true, false))
			Get();
			break;
		}
		default: SynErr(86); break;
		}
	}}

	void MimicsSpec‿NT() {
		using(astbuilder.createBarrier(""))
		{
		addAlt(58); // ALT
		addAlt(59); // ALT
		addAlt(60); // ALT
		addAlt(61); // ALT
		if (isKind(la, 58)) {
			Query‿NT();
		} else if (isKind(la, 59)) {
			Txt‿NT();
		} else if (isKind(la, 60)) {
			XL‿NT();
		} else if (isKind(la, 61)) {
			Ref‿NT();
		} else SynErr(87);
	}}

	void Query‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(58); // T
		Expect(58); // "query"
		addAlt(11); // T
		Expect(11); // colon
		addAlt(2); // T
		Expect(2); // dottedident
		addAlt(9); // T
		Expect(9); // dot
		addAlt(1); // T
		Expect(1); // ident
		addAlt(11); // T
		Expect(11); // colon
		StringOrIdent‿NT();
	}}

	void Txt‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(59); // T
		Expect(59); // "txt"
		addAlt(11); // T
		Expect(11); // colon
		addAlt(2); // T
		Expect(2); // dottedident
		addAlt(9); // T
		Expect(9); // dot
		addAlt(1); // T
		Expect(1); // ident
		addAlt(11); // T
		Expect(11); // colon
		StringOrIdent‿NT();
	}}

	void XL‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(60); // T
		Expect(60); // "xl"
		addAlt(11); // T
		Expect(11); // colon
		addAlt(2); // T
		Expect(2); // dottedident
		addAlt(9); // T
		Expect(9); // dot
		addAlt(1); // T
		Expect(1); // ident
		addAlt(11); // T
		Expect(11); // colon
		StringOrIdent‿NT();
	}}

	void Ref‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(61); // T
		Expect(61); // "ref"
		addAlt(11); // T
		Expect(11); // colon
		addAlt(19); // ALT
		addAlt(20); // ALT
		if (isKind(la, 19)) {
			Get();
		} else if (isKind(la, 20)) {
			Get();
		} else SynErr(88);
		addAlt(11); // T
		Expect(11); // colon
		StringOrIdent‿NT();
	}}

	void StringOrIdent‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(5); // ALT
		addAlt(new int[] {1, 2}); // ALT
		if (isKind(la, 5)) {
			Get();
		} else if (isKind(la, 1) || isKind(la, 2)) {
			DottedIdentBare‿NT();
		} else SynErr(89);
	}}

	void SSCommands‿NT() {
		using(astbuilder.createBarrier(null))
		{
		using(astbuilder.createMarker(null, null, true, true, false))
		SSCommand‿NT();
		addAlt(10); // ITER start
		while (isKind(la, 10)) {
			Get();
			using(astbuilder.createMarker(null, null, true, true, false))
			SSCommand‿NT();
			addAlt(10); // ITER end
		}
	}}

	void SSCommand‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(14); // ALT
		addAlt(15); // ALT
		addAlt(16); // ALT
		addAlt(17); // ALT
		addAlt(18); // ALT
		if (isKind(la, 14)) {
			Get();
		} else if (isKind(la, 15)) {
			Get();
		} else if (isKind(la, 16)) {
			Get();
		} else if (isKind(la, 17)) {
			Get();
		} else if (isKind(la, 18)) {
			Get();
		} else SynErr(90);
	}}

	void EnumValue‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(1); // T
		Expect(1); // ident
		addAlt(37); // OPT
		if (isKind(la, 37)) {
			EnumIntValue‿NT();
		}
	}}

	void EnumValues‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(1); // ITER start
		while (isKind(la, 1)) {
			EnumValue‿NT();
			addAlt(1); // ITER end
		}
		addAlt(71); // T
		Expect(71); // "default"
		EnumValue‿NT();
		addAlt(1); // ITER start
		while (isKind(la, 1)) {
			EnumValue‿NT();
			addAlt(1); // ITER end
		}
	}}

	void EnumIntValue‿NT() {
		using(astbuilder.createBarrier(null))
		{
		addAlt(37); // T
		Expect(37); // "="
		addAlt(4); // T
		Expect(4); // int
	}}



		public override void Parse() 
		{
			la = Token.Zero;
			Get();
		WFModel‿NT();
		Expect(0);
		types.CheckDeclared();
		enumtypes.CheckDeclared();
		
		}
	
		// a token's base type
		public static readonly int[] tBase = {
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1, 1, 1, 1,  1, 1, 1, 1,
		 1, 1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1
		};

		// a token's name
		public static readonly string[] tName = {
		"EOF","ident","dottedident","number", "int","string","braced","bracketed", "\"end\"","\".\"","\"|\"","\":\"", "versionnumber","\"version\"","\"search\"","\"select\"", "\"details\"","\"edit\"","\"clear\"","\"keys\"",
		"\"displayname\"","vbident","\"namespace\"","\"readerwriterprefix\"", "\"rootclass\"","\"data\"","\"class\"","\"via\"", "\"inherits\"","\"property\"","\"infoproperty\"","\"approperty\"", "\"list\"","\"selectlist\"","\"flagslist\"","\"longproperty\"", "\"infolongproperty\"","\"=\"","\"true\"","\"false\"",
		"\"#\"","\"as\"","\"double\"","\"date\"", "\"datetime\"","\"integer\"","\"percent\"","\"percentwithdefault\"", "\"doublewithdefault\"","\"integerwithdefault\"","\"n2\"","\"n0\"", "\"string\"","\"boolean\"","\"guid\"","\"string()\"", "\"xml\"","\"mimics\"","\"query\"","\"txt\"",
		"\"xl\"","\"ref\"","\"subsystem\"","\"ssname\"", "\"ssconfig\"","\"sstyp\"","\"sscommands\"","\"sskey\"", "\"ssclear\"","\"flags\"","\"enum\"","\"default\"", "???"
		};
		public override string NameOf(int tokenKind) => tName[tokenKind];

		// states that a particular production (1st index) can start with a particular token (2nd index)
		static readonly bool[,] set0 = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_T,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x}

		};

		// as set0 but with token inheritance taken into account
		static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_T,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_T,_T,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x}

		};



        public readonly AST.Builder astbuilder; // can also be private  
        public AST ast { get { return astbuilder.current; }}




		private class Errors : ErrorsBase
		{
			public override void SynErr(int line, int col, int n) 
			{
				string s;
				switch (n) 
				{
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "dottedident expected"; break;
			case 3: s = "number expected"; break;
			case 4: s = "int expected"; break;
			case 5: s = "string expected"; break;
			case 6: s = "braced expected"; break;
			case 7: s = "bracketed expected"; break;
			case 8: s = "end expected"; break;
			case 9: s = "dot expected"; break;
			case 10: s = "bar expected"; break;
			case 11: s = "colon expected"; break;
			case 12: s = "versionnumber expected"; break;
			case 13: s = "version expected"; break;
			case 14: s = "search expected"; break;
			case 15: s = "select expected"; break;
			case 16: s = "details expected"; break;
			case 17: s = "edit expected"; break;
			case 18: s = "clear expected"; break;
			case 19: s = "keys expected"; break;
			case 20: s = "displayname expected"; break;
			case 21: s = "vbident expected"; break;
			case 22: s = "\"namespace\" expected"; break;
			case 23: s = "\"readerwriterprefix\" expected"; break;
			case 24: s = "\"rootclass\" expected"; break;
			case 25: s = "\"data\" expected"; break;
			case 26: s = "\"class\" expected"; break;
			case 27: s = "\"via\" expected"; break;
			case 28: s = "\"inherits\" expected"; break;
			case 29: s = "\"property\" expected"; break;
			case 30: s = "\"infoproperty\" expected"; break;
			case 31: s = "\"approperty\" expected"; break;
			case 32: s = "\"list\" expected"; break;
			case 33: s = "\"selectlist\" expected"; break;
			case 34: s = "\"flagslist\" expected"; break;
			case 35: s = "\"longproperty\" expected"; break;
			case 36: s = "\"infolongproperty\" expected"; break;
			case 37: s = "\"=\" expected"; break;
			case 38: s = "\"true\" expected"; break;
			case 39: s = "\"false\" expected"; break;
			case 40: s = "\"#\" expected"; break;
			case 41: s = "\"as\" expected"; break;
			case 42: s = "\"double\" expected"; break;
			case 43: s = "\"date\" expected"; break;
			case 44: s = "\"datetime\" expected"; break;
			case 45: s = "\"integer\" expected"; break;
			case 46: s = "\"percent\" expected"; break;
			case 47: s = "\"percentwithdefault\" expected"; break;
			case 48: s = "\"doublewithdefault\" expected"; break;
			case 49: s = "\"integerwithdefault\" expected"; break;
			case 50: s = "\"n2\" expected"; break;
			case 51: s = "\"n0\" expected"; break;
			case 52: s = "\"string\" expected"; break;
			case 53: s = "\"boolean\" expected"; break;
			case 54: s = "\"guid\" expected"; break;
			case 55: s = "\"string()\" expected"; break;
			case 56: s = "\"xml\" expected"; break;
			case 57: s = "\"mimics\" expected"; break;
			case 58: s = "\"query\" expected"; break;
			case 59: s = "\"txt\" expected"; break;
			case 60: s = "\"xl\" expected"; break;
			case 61: s = "\"ref\" expected"; break;
			case 62: s = "\"subsystem\" expected"; break;
			case 63: s = "\"ssname\" expected"; break;
			case 64: s = "\"ssconfig\" expected"; break;
			case 65: s = "\"sstyp\" expected"; break;
			case 66: s = "\"sscommands\" expected"; break;
			case 67: s = "\"sskey\" expected"; break;
			case 68: s = "\"ssclear\" expected"; break;
			case 69: s = "\"flags\" expected"; break;
			case 70: s = "\"enum\" expected"; break;
			case 71: s = "\"default\" expected"; break;
			case 72: s = "??? expected"; break;
			case 73: s = "this symbol not expected in Namespace"; break;
			case 74: s = "this symbol not expected in ReaderWriterPrefix"; break;
			case 75: s = "this symbol not expected in RootClass"; break;
			case 76: s = "this symbol not expected in Class"; break;
			case 77: s = "this symbol not expected in SubSystem"; break;
			case 78: s = "this symbol not expected in Enum"; break;
			case 79: s = "this symbol not expected in Flags"; break;
			case 80: s = "this symbol not expected in Prop"; break;
			case 81: s = "invalid Prop"; break;
			case 82: s = "invalid Type"; break;
			case 83: s = "invalid As"; break;
			case 84: s = "invalid Mimics"; break;
			case 85: s = "invalid InitValue"; break;
			case 86: s = "invalid BaseType"; break;
			case 87: s = "invalid MimicsSpec"; break;
			case 88: s = "invalid Ref"; break;
			case 89: s = "invalid StringOrIdent"; break;
			case 90: s = "invalid SSCommand"; break;

					default: s = "error " + n; break;
				}
				// public void Add(int id, int level, int line, int col, string message)
				Add(SynErrOffset + n, ErrorLevel, line, col, s);
			}
		} // Errors

	} // end Parser

// end namespace implicit
}