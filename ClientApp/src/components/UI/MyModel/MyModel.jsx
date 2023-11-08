import React from "react";
import cl from './MyModel.module.css';

const MyModel = ({children, visible, setVisible}) => {
    const rootClasses = [cl.MyModel]
    if (visible) {
        rootClasses.push(cl.active);
    }

    return (
        <div className={rootClasses.join(' ')} onClick={() => setVisible(false)}>
            <div className={cl.MyModelContent} onClick={(e => e.stopPropagation())}>
                {children}
            </div>
        </div>
    )
}

export default MyModel;