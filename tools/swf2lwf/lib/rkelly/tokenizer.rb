require 'rkelly/lexeme'

module RKelly
  class Tokenizer
    KEYWORDS = %w{
      break case catch continue default delete do else finally for function
      if in instanceof new return switch this throw try typeof var void while 
      with 

      const true false null debugger
    }

    RESERVED = %w{
      abstract boolean byte char class double enum export extends
      final float goto implements import int interface long native package
      private protected public short static super synchronized throws
      transient volatile
    }

    LITERALS = {
      # Punctuators
      '=='  => :EQEQ,
      '!='  => :NE,
      '===' => :STREQ,
      '!==' => :STRNEQ,
      '<='  => :LE,
      '>='  => :GE,
      '||'  => :OR,
      '&&'  => :AND,
      '++'  => :PLUSPLUS,
      '--'  => :MINUSMINUS,
      '<<'  => :LSHIFT,
      '<<=' => :LSHIFTEQUAL,
      '>>'  => :RSHIFT,
      '>>=' => :RSHIFTEQUAL,
      '>>>' => :URSHIFT,
      '>>>='=> :URSHIFTEQUAL,
      '&='  => :ANDEQUAL,
      '%='  => :MODEQUAL,
      '^='  => :XOREQUAL,
      '|='  => :OREQUAL,
      '+='  => :PLUSEQUAL,
      '-='  => :MINUSEQUAL,
      '*='  => :MULTEQUAL,
      '/='  => :DIVEQUAL,
    }

    # Some keywords can be followed by regular expressions (eg, return and throw).
    # Others can be followed by division.
    KEYWORDS_THAT_IMPLY_DIVISION = %w{
      this true false null
    }

    KEYWORDS_THAT_IMPLY_REGEX = KEYWORDS - KEYWORDS_THAT_IMPLY_DIVISION

    SINGLE_CHARS_THAT_IMPLY_DIVISION = [')', ']', '}']

    def initialize(&block)
      @lexemes = []

      token(:COMMENT, /\A\/(?:\*(?:.)*?\*\/|\/[^\n]*)/m)
      token(:STRING, /\A"(?:[^"\\]*(?:\\.[^"\\]*)*)"|\A'(?:[^'\\]*(?:\\.[^'\\]*)*)'/m)

      # A regexp to match floating point literals (but not integer literals).
      token(:NUMBER, /\A\d+\.\d*(?:[eE][-+]?\d+)?|\A\d+(?:\.\d*)?[eE][-+]?\d+|\A\.\d+(?:[eE][-+]?\d+)?/m) do |type, value|
        value.gsub!(/\.(\D)/, '.0\1') if value =~ /\.\w/
        value.gsub!(/\.$/, '.0') if value =~ /\.$/
        value.gsub!(/^\./, '0.') if value =~ /^\./
        [type, eval(value)]
      end
      token(:NUMBER, /\A0[xX][\da-fA-F]+|\A0[0-7]*|\A\d+/) do |type, value|
        [type, eval(value)]
      end

      token(:LITERALS,
        Regexp.new(LITERALS.keys.sort_by { |x|
          x.length
        }.reverse.map { |x| "\\A#{x.gsub(/([|+*^])/, '\\\\\1')}" }.join('|')
      )) do |type, value|
        [LITERALS[value], value]
      end

      token(:RAW_IDENT, /\A([@\$]*[_\$A-Za-z][_\$0-9A-Za-z]*)/) do |type,value|
        if KEYWORDS.include?(value)
          [value.upcase.to_sym, value]
        elsif RESERVED.include?(value)
          [:RESERVED, value]
        else
          [:IDENT, value]
        end
      end

      # To distinguish regular expressions from comments, we require that
      # regular expressions start with a non * character (ie, not look like
      # /*foo*/). Note that we can't depend on the length of the match to
      # correctly distinguish, since `/**/i` is longer if matched as a regular
      # expression than as matched as a comment.
      # Incidentally, we're also not matching empty regular expressions
      # (eg, // and //g). Here we could depend on match length and priority to
      # determine that these are actually comments, but it turns out to be
      # easier to not match them in the first place.
      token(:REGEXP, /\A\/(?:[^\/\r\n\\*]|\\[^\r\n])[^\/\r\n\\]*(?:\\[^\r\n][^\/\r\n\\]*)*\/[gim]*/)
      token(:S, /\A[\s\r\n]*/m)

      token(:SINGLE_CHAR, /\A./) do |type, value|
        [value, value]
      end
    end

    def tokenize(string)
      raw_tokens(string).map { |x| x.to_racc_token }
    end

    def raw_tokens(string)
      tokens = []
      line_number = 1
      accepting_regexp = true
      offset = 0
      while string.length > 0
        longest_token = nil

        @lexemes.each { |lexeme|
          next if lexeme.name == :REGEXP && !accepting_regexp

          match = lexeme.match(string)
          next if match.nil?
          longest_token = match if longest_token.nil?
          next if longest_token.value.length >= match.value.length
          longest_token = match
        }

        if longest_token.name != :S
          accepting_regexp = followable_by_regex(longest_token)
        end

        longest_token.offset = offset
        longest_token.line = line_number
        line_number += longest_token.value.scan(/\n/).length
        offset += longest_token.value.length
        string = string.slice(Range.new(longest_token.value.length, -1))
        tokens << longest_token
      end
      tokens
    end
  
    private
    def token(name, pattern = nil, &block)
      @lexemes << Lexeme.new(name, pattern, &block)
    end

    def followable_by_regex(current_token)
      case current_token.name
      when :RAW_IDENT
        KEYWORDS_THAT_IMPLY_REGEX.include?(current_token.value)
      when :NUMBER
        false
      when :SINGLE_CHAR
        !SINGLE_CHARS_THAT_IMPLY_DIVISION.include?(current_token.value)
      else
        true
      end
    end
  end
end
