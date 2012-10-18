require 'strscan'

module HTML #:nodoc:
  
  # A simple HTML tokenizer. It simply breaks a stream of text into tokens, where each
  # token is a string. Each string represents either "text", or an HTML element.
  #
  # This currently assumes valid XHTML, which means no free < or > characters.
  #
  # Usage:
  #
  #   tokenizer = HTML::Tokenizer.new(text)
  #   while token = tokenizer.next
  #     p token
  #   end
  class Tokenizer #:nodoc:
    
    # The current (byte) position in the text
    attr_reader :position
    
    # The current line number
    attr_reader :line
    
    # Create a new Tokenizer for the given text.
    def initialize(text)
      @scanner = StringScanner.new(text)
      @position = 0
      @line = 0
      @current_line = 1
    end

    # Return the next token in the sequence, or +nil+ if there are no more tokens in
    # the stream.
    def next
      return nil if @scanner.eos?
      @position = @scanner.pos
      @line = @current_line
      if @scanner.check(/<\S/)
        update_current_line(scan_tag)
      else
        update_current_line(scan_text)
      end
    end
  
    private

      # Treat the text at the current position as a tag, and scan it. Supports
      # comments, doctype tags, and regular tags, and ignores less-than and
      # greater-than characters within quoted strings.
      def scan_tag
        tag = @scanner.getch
        if @scanner.scan(/!--/) # comment
          tag << @scanner.matched
          tag << (@scanner.scan_until(/--\s*>/) || @scanner.scan_until(/\Z/))
        elsif @scanner.scan(/!\[CDATA\[/)
          tag << @scanner.matched
          tag << @scanner.scan_until(/\]\]>/)
        elsif @scanner.scan(/!/) # doctype
          tag << @scanner.matched
          tag << consume_quoted_regions
        else
          tag << consume_quoted_regions
        end
        tag
      end

      # Scan all text up to the next < character and return it.
      def scan_text
        "#{@scanner.getch}#{@scanner.scan(/[^<]*/)}"
      end
      
      # Counts the number of newlines in the text and updates the current line
      # accordingly.
      def update_current_line(text)
        text.scan(/\r?\n/) { @current_line += 1 }
      end
      
      # Skips over quoted strings, so that less-than and greater-than characters
      # within the strings are ignored.
      def consume_quoted_regions
        text = ""
        loop do
          match = @scanner.scan_until(/['"<>]/) or break

          delim = @scanner.matched
          if delim == "<"
            match = match.chop
            @scanner.pos -= 1
          end

          text << match
          break if delim == "<" || delim == ">"

          # consume the quoted region
          while match = @scanner.scan_until(/[\\#{delim}]/)
            text << match
            break if @scanner.matched == delim
            text << @scanner.getch # skip the escaped character
          end
        end
        text
      end
  end
  
end

module HTML #:nodoc:
  
  class Conditions < Hash #:nodoc:
    def initialize(hash)
      super()
      hash = { :content => hash } unless Hash === hash
      hash = keys_to_symbols(hash)
      hash.each do |k,v|
        case k
          when :tag, :content then
            # keys are valid, and require no further processing
          when :attributes then
            hash[k] = keys_to_strings(v)
          when :parent, :child, :ancestor, :descendant, :sibling, :before,
                  :after
            hash[k] = Conditions.new(v)
          when :children
            hash[k] = v = keys_to_symbols(v)
            v.each do |k,v2|
              case k
                when :count, :greater_than, :less_than
                  # keys are valid, and require no further processing
                when :only
                  v[k] = Conditions.new(v2)
                else
                  raise "illegal key #{k.inspect} => #{v2.inspect}"
              end
            end
          else
            raise "illegal key #{k.inspect} => #{v.inspect}"
        end
      end
      update hash
    end

    private

      def keys_to_strings(hash)
        hash.keys.inject({}) do |h,k|
          h[k.to_s] = hash[k]
          h
        end
      end

      def keys_to_symbols(hash)
        hash.keys.inject({}) do |h,k|
          raise "illegal key #{k.inspect}" unless k.respond_to?(:to_sym)
          h[k.to_sym] = hash[k]
          h
        end
      end
  end

  # The base class of all nodes, textual and otherwise, in an HTML document.
  class Node #:nodoc:
    # The array of children of this node. Not all nodes have children.
    attr_reader :children
    
    # The parent node of this node. All nodes have a parent, except for the
    # root node.
    attr_reader :parent
    
    # The line number of the input where this node was begun
    attr_reader :line
    
    # The byte position in the input where this node was begun
    attr_reader :position
    
    # Create a new node as a child of the given parent.
    def initialize(parent, line=0, pos=0)
      @parent = parent
      @children = []
      @line, @position = line, pos
    end

    # Return a textual representation of the node.
    def to_s
      s = ""
      @children.each { |child| s << child.to_s }
      s
    end

    # Return false (subclasses must override this to provide specific matching
    # behavior.) +conditions+ may be of any type.
    def match(conditions)
      false
    end

    # Search the children of this node for the first node for which #find
    # returns non +nil+. Returns the result of the #find call that succeeded.
    def find(conditions)
      conditions = validate_conditions(conditions)

      @children.each do |child|        
        node = child.find(conditions)
        return node if node
      end
      nil
    end

    # Search for all nodes that match the given conditions, and return them
    # as an array.
    def find_all(conditions)
      conditions = validate_conditions(conditions)

      matches = []
      matches << self if match(conditions)
      @children.each do |child|
        matches.concat child.find_all(conditions)
      end
      matches
    end

    # Returns +false+. Subclasses may override this if they define a kind of
    # tag.
    def tag?
      false
    end

    def validate_conditions(conditions)
      Conditions === conditions ? conditions : Conditions.new(conditions)
    end

    def ==(node)
      return false unless self.class == node.class && children.size == node.children.size

      equivalent = true

      children.size.times do |i|
        equivalent &&= children[i] == node.children[i]
      end

      equivalent
    end
  
    class <<self
      def parse(parent, line, pos, content, strict=true)
        if content !~ /^<\S/
          Text.new(parent, line, pos, content)
        else
          scanner = StringScanner.new(content)

          unless scanner.skip(/</)
            if strict
              raise "expected <"
            else
              return Text.new(parent, line, pos, content)
            end
          end

          if scanner.skip(/!\[CDATA\[/)
            scanner.scan_until(/\]\]>/)
            return CDATA.new(parent, line, pos, scanner.pre_match)
          end
          
          closing = ( scanner.scan(/\//) ? :close : nil )
          return Text.new(parent, line, pos, content) unless name = scanner.scan(/[\w:]+/)
          name.downcase!
  
          unless closing
            scanner.skip(/\s*/)
            attributes = {}
            while attr = scanner.scan(/[-\w:]+/)
              value = true
              if scanner.scan(/\s*=\s*/)
                if delim = scanner.scan(/['"]/)
                  value = ""
                  while text = scanner.scan(/[^#{delim}\\]+|./)
                    case text
                      when "\\" then
                        value << text
                        value << scanner.getch
                      when delim
                        break
                      else value << text
                    end
                  end
                else
                  value = scanner.scan(/[^\s>\/]+/)
                end
              end
              attributes[attr.downcase] = value
              scanner.skip(/\s*/)
            end
    
            closing = ( scanner.scan(/\//) ? :self : nil )
          end
          
          unless scanner.scan(/\s*>/)
            if strict
              raise "expected > (got #{scanner.rest.inspect} for #{content}, #{attributes.inspect})" 
            else
              # throw away all text until we find what we're looking for
              scanner.skip_until(/>/) or scanner.terminate
            end
          end

          Tag.new(parent, line, pos, name, attributes, closing)
        end
      end
    end
  end

  # A node that represents text, rather than markup.
  class Text < Node #:nodoc:
    
    attr_reader :content
    
    # Creates a new text node as a child of the given parent, with the given
    # content.
    def initialize(parent, line, pos, content)
      super(parent, line, pos)
      @content = content
    end

    # Returns the content of this node.
    def to_s
      @content
    end

    # Returns +self+ if this node meets the given conditions. Text nodes support
    # conditions of the following kinds:
    #
    # * if +conditions+ is a string, it must be a substring of the node's
    #   content
    # * if +conditions+ is a regular expression, it must match the node's
    #   content
    # * if +conditions+ is a hash, it must contain a <tt>:content</tt> key that
    #   is either a string or a regexp, and which is interpreted as described
    #   above.
    def find(conditions)
      match(conditions) && self
    end
    
    # Returns non-+nil+ if this node meets the given conditions, or +nil+
    # otherwise. See the discussion of #find for the valid conditions.
    def match(conditions)
      case conditions
        when String
          @content.index(conditions)
        when Regexp
          @content =~ conditions
        when Hash
          conditions = validate_conditions(conditions)

          # Text nodes only have :content, :parent, :ancestor
          unless (conditions.keys - [:content, :parent, :ancestor]).empty?
            return false
          end

          match(conditions[:content])
        else
          nil
      end
    end

    def ==(node)
      return false unless super
      content == node.content
    end
  end
  
  # A CDATA node is simply a text node with a specialized way of displaying
  # itself.
  class CDATA < Text #:nodoc:
    def to_s
      "<![CDATA[#{super}]>"
    end
  end

  # A Tag is any node that represents markup. It may be an opening tag, a
  # closing tag, or a self-closing tag. It has a name, and may have a hash of
  # attributes.
  class Tag < Node #:nodoc:
    
    # Either +nil+, <tt>:close</tt>, or <tt>:self</tt>
    attr_reader :closing
    
    # Either +nil+, or a hash of attributes for this node.
    attr_reader :attributes

    # The name of this tag.
    attr_reader :name
        
    # Create a new node as a child of the given parent, using the given content
    # to describe the node. It will be parsed and the node name, attributes and
    # closing status extracted.
    def initialize(parent, line, pos, name, attributes, closing)
      super(parent, line, pos)
      @name = name
      @attributes = attributes
      @closing = closing
    end

    # A convenience for obtaining an attribute of the node. Returns +nil+ if
    # the node has no attributes.
    def [](attr)
      @attributes ? @attributes[attr] : nil
    end

    # Returns non-+nil+ if this tag can contain child nodes.
    def childless?(xml = false)
      return false if xml && @closing.nil?
      !@closing.nil? ||
        @name =~ /^(img|br|hr|link|meta|area|base|basefont|
                    col|frame|input|isindex|param)$/ox
    end

    # Returns a textual representation of the node
    def to_s
      if @closing == :close
        "</#{@name}>"
      else
        s = "<#{@name}"
        @attributes.each do |k,v|
          s << " #{k}"
          s << "='#{v.gsub(/'/,"\\\\'")}'" if String === v
        end
        s << " /" if @closing == :self
        s << ">"
        @children.each { |child| s << child.to_s }
        s << "</#{@name}>" if @closing != :self && !@children.empty?
        s
      end
    end

    # If either the node or any of its children meet the given conditions, the
    # matching node is returned. Otherwise, +nil+ is returned. (See the
    # description of the valid conditions in the +match+ method.)
    def find(conditions)
      match(conditions) && self || super
    end

    # Returns +true+, indicating that this node represents an HTML tag.
    def tag?
      true
    end
    
    # Returns +true+ if the node meets any of the given conditions. The
    # +conditions+ parameter must be a hash of any of the following keys
    # (all are optional):
    #
    # * <tt>:tag</tt>: the node name must match the corresponding value
    # * <tt>:attributes</tt>: a hash. The node's values must match the
    #   corresponding values in the hash.
    # * <tt>:parent</tt>: a hash. The node's parent must match the
    #   corresponding hash.
    # * <tt>:child</tt>: a hash. At least one of the node's immediate children
    #   must meet the criteria described by the hash.
    # * <tt>:ancestor</tt>: a hash. At least one of the node's ancestors must
    #   meet the criteria described by the hash.
    # * <tt>:descendant</tt>: a hash. At least one of the node's descendants
    #   must meet the criteria described by the hash.
    # * <tt>:sibling</tt>: a hash. At least one of the node's siblings must
    #   meet the criteria described by the hash.
    # * <tt>:after</tt>: a hash. The node must be after any sibling meeting
    #   the criteria described by the hash, and at least one sibling must match.
    # * <tt>:before</tt>: a hash. The node must be before any sibling meeting
    #   the criteria described by the hash, and at least one sibling must match.
    # * <tt>:children</tt>: a hash, for counting children of a node. Accepts the
    #   keys:
    # ** <tt>:count</tt>: either a number or a range which must equal (or
    #    include) the number of children that match.
    # ** <tt>:less_than</tt>: the number of matching children must be less than
    #    this number.
    # ** <tt>:greater_than</tt>: the number of matching children must be
    #    greater than this number.
    # ** <tt>:only</tt>: another hash consisting of the keys to use
    #    to match on the children, and only matching children will be
    #    counted.
    #
    # Conditions are matched using the following algorithm:
    #
    # * if the condition is a string, it must be a substring of the value.
    # * if the condition is a regexp, it must match the value.
    # * if the condition is a number, the value must match number.to_s.
    # * if the condition is +true+, the value must not be +nil+.
    # * if the condition is +false+ or +nil+, the value must be +nil+.
    #
    # Usage:
    #
    #   # test if the node is a "span" tag
    #   node.match :tag => "span"
    #
    #   # test if the node's parent is a "div"
    #   node.match :parent => { :tag => "div" }
    #
    #   # test if any of the node's ancestors are "table" tags
    #   node.match :ancestor => { :tag => "table" }
    #
    #   # test if any of the node's immediate children are "em" tags
    #   node.match :child => { :tag => "em" }
    #
    #   # test if any of the node's descendants are "strong" tags
    #   node.match :descendant => { :tag => "strong" }
    #
    #   # test if the node has between 2 and 4 span tags as immediate children
    #   node.match :children => { :count => 2..4, :only => { :tag => "span" } } 
    #
    #   # get funky: test to see if the node is a "div", has a "ul" ancestor
    #   # and an "li" parent (with "class" = "enum"), and whether or not it has
    #   # a "span" descendant that contains # text matching /hello world/:
    #   node.match :tag => "div",
    #              :ancestor => { :tag => "ul" },
    #              :parent => { :tag => "li",
    #                           :attributes => { :class => "enum" } },
    #              :descendant => { :tag => "span",
    #                               :child => /hello world/ }
    def match(conditions)
      conditions = validate_conditions(conditions)

      # check content of child nodes
      if conditions[:content]
        if children.empty?
          return false unless match_condition("", conditions[:content])
        else
          return false unless children.find { |child| child.match(conditions[:content]) }
        end
      end

      # test the name
      return false unless match_condition(@name, conditions[:tag]) if conditions[:tag]

      # test attributes
      (conditions[:attributes] || {}).each do |key, value|
        return false unless match_condition(self[key], value)
      end

      # test parent
      return false unless parent.match(conditions[:parent]) if conditions[:parent]

      # test children
      return false unless children.find { |child| child.match(conditions[:child]) } if conditions[:child]
   
      # test ancestors
      if conditions[:ancestor]
        return false unless catch :found do
          p = self
          throw :found, true if p.match(conditions[:ancestor]) while p = p.parent
        end
      end

      # test descendants
      if conditions[:descendant]
        return false unless children.find do |child|
          # test the child
          child.match(conditions[:descendant]) ||
          # test the child's descendants
          child.match(:descendant => conditions[:descendant])
        end
      end
      
      # count children
      if opts = conditions[:children]
        matches = children.select do |c|
          c.match(/./) or
          (c.kind_of?(HTML::Tag) and (c.closing == :self or ! c.childless?))
        end
        
        matches = matches.select { |c| c.match(opts[:only]) } if opts[:only]
        opts.each do |key, value|
          next if key == :only
          case key
            when :count
              if Integer === value
                return false if matches.length != value
              else
                return false unless value.include?(matches.length)
              end
            when :less_than
              return false unless matches.length < value
            when :greater_than
              return false unless matches.length > value
            else raise "unknown count condition #{key}"
          end
        end
      end

      # test siblings
      if conditions[:sibling] || conditions[:before] || conditions[:after]
        siblings = parent ? parent.children : []
        self_index = siblings.index(self)

        if conditions[:sibling]
          return false unless siblings.detect do |s| 
            s != self && s.match(conditions[:sibling])
          end
        end

        if conditions[:before]
          return false unless siblings[self_index+1..-1].detect do |s| 
            s != self && s.match(conditions[:before])
          end
        end

        if conditions[:after]
          return false unless siblings[0,self_index].detect do |s| 
            s != self && s.match(conditions[:after])
          end
        end
      end
  
      true
    end

    def ==(node)
      return true if equal?(node)
      return false unless super
      return false unless closing == node.closing && self.name == node.name
      attributes == node.attributes
    end
    
    private
      # Match the given value to the given condition.
      def match_condition(value, condition)
        case condition
          when String
            value && value == condition
          when Regexp
            value && value.match(condition)
          when Numeric
            value == condition.to_s
          when true
            !value.nil?
          when false, nil
            value.nil?
          else
            false
        end
      end
  end
end
module HTML #:nodoc:
  
  # A top-level HTMl document. You give it a body of text, and it will parse that
  # text into a tree of nodes.
  class Document #:nodoc:

    # The root of the parsed document.
    attr_reader :root

    # Create a new Document from the given text.
    def initialize(text, strict=false, xml=false)
      tokenizer = Tokenizer.new(text)
      @root = Node.new(nil)
      node_stack = [ @root ]
      while token = tokenizer.next
        node = Node.parse(node_stack.last, tokenizer.line, tokenizer.position, token, strict)

        node_stack.last.children << node unless node.tag? && node.closing == :close
        if node.tag?
          if node_stack.length > 1 && node.closing == :close
            if node_stack.last.name == node.name
              node_stack.pop
            else
              open_start = node_stack.last.position - 20
              open_start = 0 if open_start < 0
              close_start = node.position - 20
              close_start = 0 if close_start < 0
              msg = <<EOF.strip
ignoring attempt to close #{node_stack.last.name} with #{node.name}
  opened at byte #{node_stack.last.position}, line #{node_stack.last.line}
  closed at byte #{node.position}, line #{node.line}
  attributes at open: #{node_stack.last.attributes.inspect}
  text around open: #{text[open_start,40].inspect}
  text around close: #{text[close_start,40].inspect}
EOF
              strict ? raise(msg) : warn(msg)
            end
          elsif !node.childless?(xml) && node.closing != :close
            node_stack.push node
          end
        end
      end
    end
  
    # Search the tree for (and return) the first node that matches the given
    # conditions. The conditions are interpreted differently for different node
    # types, see HTML::Text#find and HTML::Tag#find.
    def find(conditions)
      @root.find(conditions)
    end

    # Search the tree for (and return) all nodes that match the given
    # conditions. The conditions are interpreted differently for different node
    # types, see HTML::Text#find and HTML::Tag#find.
    def find_all(conditions)
      @root.find_all(conditions)
    end
    
  end

end

module HTML #:nodoc:

    # A parser for SGML, using the derived class as static DTD.
    
    class SGMLParser
    
    # Regular expressions used for parsing:
    Interesting = /[&<]/
    Incomplete = Regexp.compile('&([a-zA-Z][a-zA-Z0-9]*|#[0-9]*)?|' +
                                '<([a-zA-Z][^<>]*|/([a-zA-Z][^<>]*)?|' +
                                '![^<>]*)?')
    
    Entityref = /&([a-zA-Z][-.a-zA-Z0-9]*)[^-.a-zA-Z0-9]/
    Charref = /&#([0-9]+)[^0-9]/
    
    Starttagopen = /<[>a-zA-Z]/
    Endtagopen = /<\/[<>a-zA-Z]/
    # Assaf: fixed to allow tag to close itself (XHTML)
    Endbracket = /<|>|\/>/
    Special = /<![^<>]*>/
    Commentopen = /<!--/
    Commentclose = /--[ \t\n]*>/
    Tagfind = /[a-zA-Z][a-zA-Z0-9.-]*/
    # Assaf: / is no longer part of allowed attribute value
    Attrfind = Regexp.compile('[\s,]*([a-zA-Z_][a-zA-Z_0-9.-]*)' +
                                '(\s*=\s*' +
                                "('[^']*'" +
                                '|"[^"]*"' +
                                '|[-~a-zA-Z0-9,.:+*%?!()_#=]*))?')
    
    Entitydefs =
        {'lt'=>'<', 'gt'=>'>', 'amp'=>'&', 'quot'=>'"', 'apos'=>'\''}
    
    def initialize(verbose=false)
        @verbose = verbose
        reset
    end
    
    def reset
        @rawdata = ''
        @stack = []
        @lasttag = '???'
        @nomoretags = false
        @literal = false
    end
    
    def has_context(gi)
        @stack.include? gi
    end
    
    def setnomoretags
        @nomoretags = true
        @literal = true
    end
    
    def setliteral(*args)
        @literal = true
    end
    
    def feed(data)
        @rawdata << data
        goahead(false)
    end
    
    def close
        goahead(true)
    end
    
    def goahead(_end)
        rawdata = @rawdata
        i = 0
        n = rawdata.length
        while i < n
        if @nomoretags
            handle_data(rawdata[i..(n-1)])
            i = n
            break
        end
        j = rawdata.index(Interesting, i)
        j = n unless j
        if i < j
            handle_data(rawdata[i..(j-1)])
        end
        i = j
        break if (i == n)
        if rawdata[i] == ?< #
            if rawdata.index(Starttagopen, i) == i
            if @literal
                handle_data(rawdata[i, 1])
                i += 1
                next
            end
            k = parse_starttag(i)
            break unless k
            i = k
            next
            end
            if rawdata.index(Endtagopen, i) == i
            k = parse_endtag(i)
            break unless k
            i = k
            @literal = false
            next
            end
            if rawdata.index(Commentopen, i) == i
            if @literal
                handle_data(rawdata[i,1])
                i += 1
                next
            end
            k = parse_comment(i)
            break unless k
            i += k
            next
            end
            if rawdata.index(Special, i) == i
            if @literal
                handle_data(rawdata[i, 1])
                i += 1
                next
            end
            k = parse_special(i)
            break unless k
            i += k
            next
            end
        elsif rawdata[i] == ?& #
            if rawdata.index(Charref, i) == i
            i += $&.length
            handle_charref($1)
            i -= 1 unless rawdata[i-1] == ?;
            next
            end
            if rawdata.index(Entityref, i) == i
            i += $&.length
            handle_entityref($1)
            i -= 1 unless rawdata[i-1] == ?;
            next
            end
        else
            raise RuntimeError, 'neither < nor & ??'
        end
        # We get here only if incomplete matches but
        # nothing else
        match = rawdata.index(Incomplete, i)
        unless match == i
            handle_data(rawdata[i, 1])
            i += 1
            next
        end
        j = match + $&.length
        break if j == n # Really incomplete
        handle_data(rawdata[i..(j-1)])
        i = j
        end
        # end while
        if _end and i < n
        handle_data(@rawdata[i..(n-1)])
        i = n
        end
        @rawdata = rawdata[i..-1]
    end
    
    def parse_comment(i)
        rawdata = @rawdata
        if rawdata[i, 4] != '<!--'
        raise RuntimeError, 'unexpected call to handle_comment'
        end
        match = rawdata.index(Commentclose, i)
        return nil unless match
        matched_length = $&.length
        j = match
        handle_comment(rawdata[i+4..(j-1)])
        j = match + matched_length
        return j-i
    end
    
    def parse_starttag(i)
        rawdata = @rawdata
        j = rawdata.index(Endbracket, i + 1)
        return nil unless j
        attrs = []
        if rawdata[i+1] == ?> #
        # SGML shorthand: <> == <last open tag seen>
        k = j
        tag = @lasttag
        else
        match = rawdata.index(Tagfind, i + 1)
        unless match
            raise RuntimeError, 'unexpected call to parse_starttag'
        end
        k = i + 1 + ($&.length)
        tag = $&.downcase
        @lasttag = tag
        end
        while k < j
        # Assaf: fixed to allow tag to close itself (XHTML)
        break unless idx = rawdata.index(Attrfind, k) and idx < j
        matched_length = $&.length
        attrname, rest, attrvalue = $1, $2, $3
        if not rest
            attrvalue = '' # was: = attrname
        # Assaf: fixed to handle double quoted attribute values properly
        elsif (attrvalue[0] == ?' && attrvalue[-1] == ?') or
            (attrvalue[0] == ?" && attrvalue[-1] == ?")
            attrvalue = attrvalue[1..-2]
        end
        attrs << [attrname.downcase, attrvalue]
        k += matched_length
        end
        # Assaf: fixed to allow tag to close itself (XHTML)
        if rawdata[j,2] == '/>'
        j += 2
        finish_starttag(tag, attrs)
        finish_endtag(tag)
        else
        if rawdata[j] == ?> #
            j += 1
        end
        finish_starttag(tag, attrs)
        end
        return j
    end
    
    def parse_endtag(i)
        rawdata = @rawdata
        j = rawdata.index(Endbracket, i + 1)
        return nil unless j
        tag = (rawdata[i+2..j-1].strip).downcase
        if rawdata[j] == ?> #
        j += 1
        end
        finish_endtag(tag)
        return j
    end
    
    def finish_starttag(tag, attrs)
        method = 'start_' + tag
        if self.respond_to?(method)
        @stack << tag
        handle_starttag(tag, method, attrs)
        return 1
        else
        method = 'do_' + tag
        if self.respond_to?(method)
            handle_starttag(tag, method, attrs)
            return 0
        else
            unknown_starttag(tag, attrs)
            return -1
        end
        end
    end
    
    def finish_endtag(tag)
        if tag == ''
        found = @stack.length - 1
        if found < 0
            unknown_endtag(tag)
            return
        end
        else
        unless @stack.include? tag
            method = 'end_' + tag
            unless self.respond_to?(method)
            unknown_endtag(tag)
            end
            return
        end
        found = @stack.index(tag) #or @stack.length
        end
        while @stack.length > found
        tag = @stack[-1]
        method = 'end_' + tag
        if respond_to?(method)
            handle_endtag(tag, method)
        else
            unknown_endtag(tag)
        end
        @stack.pop
        end
    end
    
    def parse_special(i)
        rawdata = @rawdata
        match = rawdata.index(Endbracket, i+1)
        return nil unless match
        matched_length = $&.length
        handle_special(rawdata[i+1..(match-1)])
        return match - i + matched_length
    end
    
    def handle_starttag(tag, method, attrs)
        self.send(method, attrs)
    end
    
    def handle_endtag(tag, method)
        self.send(method)
    end
    
    def report_unbalanced(tag)
        if @verbose
        print '*** Unbalanced </' + tag + '>', "\n"
        print '*** Stack:', self.stack, "\n"
        end
    end
    
    def handle_charref(name)
        n = Integer(name) rescue -1
        if !(0 <= n && n <= 255)
        unknown_charref(name)
        return
        end
        handle_data(n.chr)
    end
    
    def handle_entityref(name)
        table = Entitydefs
        if table.include?(name)
        handle_data(table[name])
        else
        unknown_entityref(name)
        return
        end
    end
    
    def handle_data(data)
    end
    
    def handle_comment(data)
    end
    
    def handle_special(data)
    end
    
    def unknown_starttag(tag, attrs)
    end
    def unknown_endtag(tag)
    end
    def unknown_charref(ref)
    end
    def unknown_entityref(ref)
    end
    
    end


    # (X)HTML parser.
    #
    # Parses a String and returns an REXML::Document with the (X)HTML content.
    #
    # For example:
    #   html = "<p>paragraph</p>"
    #   parser = HTMLParser.new(html)
    #   puts parser.document
    #
    # Requires a patched version of SGMLParser.
    class HTMLParser < SGMLParser
    
        attr :document

        def self.parse(html)
            parser = HTMLParser.new
            parser.feed(html)
            parser.document
        end
    
        def initialize()
            super
            @document = HTML::Document.new("")
            @current = @document.root
        end
    
        def handle_data(data)
            @current.children << HTML::Text.new(@current, 0, 0, data)
        end
    
        def handle_comment(data)
        end
    
        def handle_special(data)
        end
    
        def unknown_starttag(tag, attrs)
            attrs = attrs.inject({}) do |hash, attr|
                hash[attr[0].downcase] = attr[1]
                hash
            end
            element = HTML::Tag.new(@current || @document, 0, 0, tag.downcase, attrs, true)
            @current.children << element
            @current = element
        end
        
        def unknown_endtag(tag)
            @current = @current.parent if @current.parent
        end
        
        def unknown_charref(ref)
        end
        
        def unknown_entityref(ref)
            @current.children << HTML::Text.new(@current, 0, 0, "&amp;#{ref}&lt;")
        end
    
    end

end
