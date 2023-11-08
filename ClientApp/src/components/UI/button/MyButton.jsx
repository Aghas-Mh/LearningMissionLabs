import React, { useEffect, useRef, useState } from "react";
import classes from './MyButton.module.css';

const MyButton = ({children, style,  dynamicSize = false, ...props}) => {
    const [dynamicStyle, setDynamicStyle] = useState({fontSize: 15, padding: '5px 15px', height: 35});
    const buttonRef = useRef(null);

    const updateFontSize = () => {
        if (buttonRef.current) {
          const buttonWidth = buttonRef.current.offsetWidth;
          if (buttonWidth < 40) setDynamicStyle({...dynamicStyle, padding: '0px 0px', fontSize: 12})
          else if (buttonWidth < 62) setDynamicStyle({...dynamicStyle, padding: '0px 0px'});
          else setDynamicStyle({...dynamicStyle, fontSize: 15, padding: '5px 15px'});
        }
    };

    useEffect(() => {
        updateFontSize();
        if (!dynamicSize) return
        window.addEventListener('resize', updateFontSize);
    
        return () => {
          window.removeEventListener('resize', updateFontSize);
        };
    }, []);
    

    return (
        <button {...props} style={{  ...style, ...dynamicStyle }} className={classes.myBtn} ref={buttonRef}>
            {children}
        </button>
    )
}

export default MyButton;